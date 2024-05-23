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
    public partial class FormMasterImport : Form
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
        private string _saleRow;
        string Transaction_Type = null;
        private bool IsError = false;

        List<ErrorMessage> errormessage = new List<ErrorMessage>();

        #endregion

        public FormMasterImport()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public void loadFromOutside()
        {
            LoadDataGrid();
        }

        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            tableLoad();
            typeLoad();

            string singleImport = new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "BOM", null, connVM);
            singleImport = "Y";

            if (singleImport == "Y" && preSelectTable == "BOM")
            {
                dgvLoadedTable.Columns["Select"].Visible = false;

                chkSelectAll.Visible = false;

            }
        }

        public static string SelectOne(string transactionType)
        {
            FormMasterImport form = new FormMasterImport();

            form.preSelectTable = "TransferIssues";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }
        private void tableLoad()
        {
            cmbTable.Items.Add(CONST_SALETYPE);
            cmbTable.Items.Add(CONST_PURCHASETYPE);
            cmbTable.Items.Add(CONST_ISSUETYPE);
            cmbTable.Items.Add(CONST_RECEIVETYPE);
            cmbTable.Items.Add(CONST_VDSTYPE);
            cmbTable.Items.Add(CONST_BOMTYPE);
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

            CommonDAL dal = new CommonDAL();

            string value = dal.settingsDesktop("Import", "TransferImportSelection", null, connVM);

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

                            OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);
                            if (errormessage.Count > 0)
                            {
                                FormErrorMessage.ShowDetails(errormessage);
                                btnImport.Enabled = false;
                            }
                            errormessage.Clear();
                            dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_SALETYPE;
                            break;
                        case CONST_PURCHASETYPE:
                            dtTableResult = ds.Tables["PurchaseM"];

                            if (dtTableResult == null || dtTableResult.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException("Data not found. Please check SheetName", "");
                            }


                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "Invoice_Date", "Receive_Date" });

                            #region Product_Group Cloumn Check

                            if (!dtTableResult.Columns.Contains("Product_Group"))
                            {
                                //////dtTableResult.Columns.Add("Product_Group");
                                System.Data.DataColumn newColumn = new System.Data.DataColumn("Product_Group", typeof(System.String));
                                newColumn.DefaultValue = "-";
                                dtTableResult.Columns.Add(newColumn);

                            }

                            OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);
                            if (errormessage.Count > 0)
                            {
                                FormErrorMessage.ShowDetails(errormessage);
                                btnImport.Enabled = false;
                            }
                            errormessage.Clear();

                            #endregion

                            #region TransactionType Cloumn Check

                            if (!dtTableResult.Columns.Contains("Transaction_Type"))
                            {
                                MessageBox.Show(this, "TransactionType Column Required in Excel Template", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            else
                            {
                                DataView view = new DataView(dtTableResult);
                                DataTable distinctValues = view.ToTable(true, "Transaction_Type");
                                if (distinctValues.Rows.Count > 1)
                                {
                                    MessageBox.Show(this, "There Multiple TransactionType Value in Excel Template", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                else
                                {
                                    Transaction_Type = distinctValues.Rows[0]["Transaction_Type"].ToString();

                                    if (Transaction_Type != transactionType)
                                    {
                                        MessageBox.Show(this, "This Excel Template Can’t be upload here only Upload TransactionType '" + transactionType + " ' Data", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }

                                }
                            }

                            #endregion

                            #region Remove UnusedColumn
                            if (dtTableResult.Columns.Contains("Invoice_Value"))
                            {
                                dtTableResult.Columns.Remove("Invoice_Value");
                            }

                            if (dtTableResult.Columns.Contains("Exchange_Rate"))
                            {
                                dtTableResult.Columns.Remove("Exchange_Rate");
                            }
                            if (dtTableResult.Columns.Contains("Currency"))
                            {
                                dtTableResult.Columns.Remove("Currency");
                            }
                            #endregion

                            loadedTable = CONST_PURCHASETYPE;
                            this.progressBar1.Visible = true;
                            bgwExcellValidation.RunWorkerAsync();
                            // dgvLoadedTable.DataSource = dtTableResult;
                            // loadedTable = CONST_PURCHASETYPE;
                            break;
                        case CONST_ISSUETYPE:
                            dtTableResult = ds.Tables["IssueM"];

                            if (dtTableResult != null)
                            {
                                if (dtTableResult.Columns.Contains("ImportID"))
                                {
                                    dtTableResult.Columns.Remove("ImportID");
                                }
                            }

                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "Issue_DateTime" });
                            OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);

                            if (errormessage.Count > 0)
                            {
                                FormErrorMessage.ShowDetails(errormessage);
                                btnImport.Enabled = false;
                            }
                            errormessage.Clear();

                            // dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_ISSUETYPE;
                            this.progressBar1.Visible = true;
                            bgwIssueExcellValidation.RunWorkerAsync();

                            break;
                        case CONST_RECEIVETYPE:
                            dtTableResult = ds.Tables["ReceiveM"];
                            //dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "Receive_DateTime" });

                            #region Product_Group Cloumn Check

                            if (!dtTableResult.Columns.Contains("Product_Group"))
                            {
                                //////dtTableResult.Columns.Add("Product_Group");
                                System.Data.DataColumn newColumn = new System.Data.DataColumn("Product_Group", typeof(System.String));
                                newColumn.DefaultValue = "-";
                                dtTableResult.Columns.Add(newColumn);

                            }

                            #endregion

                            OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);
                            if (errormessage.Count > 0)
                            {
                                FormErrorMessage.ShowDetails(errormessage);
                                btnImport.Enabled = false;
                            }
                            errormessage.Clear();

                            loadedTable = CONST_RECEIVETYPE;

                            this.progressBar1.Visible = true;
                            bgwExcellValidation.RunWorkerAsync();

                            dtTableResult = OrdinaryVATDesktop.DtDateFormat(dtTableResult, new string[] { "Receive_DateTime" });

                            if (dtTableResult.Columns.Contains("ImportID"))
                            {
                                dtTableResult.Columns.Remove("ImportID");
                            }


                            //ReceiveDAL rDal = new ReceiveDAL();
                            //ResultVM quantityResult = rDal.ExceReceiveValidation(dtTableResult, "Quantity");
                            //ResultVM receive_dateTime = rDal.ExceReceiveValidation(dtTableResult, "Receive_DateTime");
                            //ResultVM NBR_Price = rDal.ExceReceiveValidation(dtTableResult, "NBR_Price");

                            //FormErrorMessage.ShowDetails(quantityResult.ErrorList);


                            break;
                        case CONST_VDSTYPE:
                            dtTableResult = ds.Tables["VDSM"];
                            //DataTable detailTable = ds.Tables["VDSD"];


                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "Bill_Date", "Issue_Date", "Cheque_Date", "Deposit_Date", "BankDepositDate" });
                            //dtTableResult = OrdinaryVATDesktop.DtColumnAdd(dtTableResult, "Transection_Type1", "Kamrul", "string");


                            //detailTable = OrdinaryVATDesktop.DtDateCheck(detailTable, new string[] { "Bill_Date","Issue_Date" });
                            // dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_VDSTYPE;

                            OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);
                            if (errormessage.Count > 0)
                            {
                                FormErrorMessage.ShowDetails(errormessage);
                                btnImport.Enabled = false;
                            }
                            errormessage.Clear();

                            this.progressBar1.Visible = true;
                            bgwExcellValidation.RunWorkerAsync();
                            break;
                        case CONST_BOMTYPE:
                            dtTableResult = ds.Tables["Product"];
                            loadedTable = CONST_BOMTYPE;

                            this.progressBar1.Visible = true;

                            OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);
                            if (errormessage.Count > 0)
                            {
                                FormErrorMessage.ShowDetails(errormessage);
                                btnImport.Enabled = false;

                            }
                            errormessage.Clear();
                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "EffectDate", "FirstSupplyDate" });

                            bgwExcellValidation.RunWorkerAsync();

                            //dgvLoadedTable.DataSource = dtTableResult;

                            break;
                        case CONST_TRANSFERTYPE:
                            dtTableResult = ds.Tables["TransferIssueM"];
                            dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "TransactionDateTime" });
                            #region TransactionType Cloumn Check
                            if (!dtTableResult.Columns.Contains("TransactionType"))
                            {
                                MessageBox.Show(this, "TransactionType Column Required in Excel Template", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            else
                            {
                                DataView view = new DataView(dtTableResult);
                                DataTable distinctValues = view.ToTable(true, "TransactionType");
                                if (distinctValues.Rows.Count >= 1)
                                {
                                    DataRow[] results = distinctValues.Select("TransactionType NOT IN ('FINISH','RAW','61OUT','62OUT')");
                                    if (results.Length > 0)
                                    {
                                        MessageBox.Show(this, "Wrong TransactionType is Given in Excel", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;

                                    }
                                }

                            }
                            #endregion
                            this.progressBar1.Visible = true;

                            OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);
                            if (errormessage.Count > 0)
                            {
                                FormErrorMessage.ShowDetails(errormessage);
                                btnImport.Enabled = false;
                            }
                            errormessage.Clear();

                            bgwTransferIssueExcellValidation.RunWorkerAsync();

                            // dgvLoadedTable.DataSource = dtTableResult;
                            //loadedTable = CONST_TRANSFERTYPE;
                            break;
                        case CONST_TransferReceive:
                            dtTableResult = ds.Tables["Receive"];
                            //dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "TransactionDateTime" });
                            dgvLoadedTable.DataSource = dtTableResult;

                            OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);
                            if (errormessage.Count > 0)
                            {
                                FormErrorMessage.ShowDetails(errormessage);
                                btnImport.Enabled = false;
                            }
                            errormessage.Clear();

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
                else if (selectedType == CONST_TEXT)
                {
                    if (selectedTable == CONST_PURCHASETYPE)
                    {
                        flag = commonDal.settingValue(CONST_SINGLEIMPORT, "PurchaseImport");

                        dtTableResult = flag == "Y" ? GetTableFromSingleText(CONST_PURCHASETYPE) : GetTableFromTextDouble(CONST_PURCHASETYPE);

                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_PURCHASETYPE;
                    }
                    //else if (selectedTable == CONST_SALETYPE)
                    //{
                    //    dtTableResult = GetTableFromSingleText(CONST_SALETYPE);
                    //    dgvLoadedTable.DataSource = dtTableResult;
                    //    loadedTable = CONST_SALETYPE;
                    //}
                    else if (selectedTable == CONST_ISSUETYPE)
                    {
                        dtTableResult = GetTableFromSingleText(CONST_ISSUETYPE);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_ISSUETYPE;
                    }
                    else if (selectedTable == CONST_RECEIVETYPE)
                    {
                        flag = commonDal.settingValue(CONST_SINGLEIMPORT, "ReceiveImport", connVM);

                        dtTableResult = flag == "Y" ? GetTableFromSingleText(CONST_RECEIVETYPE) : GetTableFromTextDouble(CONST_RECEIVETYPE);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_RECEIVETYPE;
                    }
                    else if (selectedTable == CONST_TRANSFERTYPE)
                    {
                        dtTableResult = GetTableFromSingleText(CONST_TRANSFERTYPE);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_TRANSFERTYPE;
                    }
                    else if (selectedTable == CONST_BOMTYPE)
                    {
                        dtTableResult = GetTableFromText(CONST_BOMTYPE);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_BOMTYPE;
                    }
                    else if (selectedTable == CONST_VDSTYPE)
                    {
                        dtTableResult = GetTableFromText(CONST_VDSTYPE);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_VDSTYPE;
                    }
                    else
                    {
                    }
                }
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
        public static DataTable EmptyRowCheckAndDeleteX(DataTable table, List<ErrorMessage> Errorlists)
        {
            //bool rowDel = false;
            //ErrorMessage Errorlist = new ErrorMessage();
            //for (int i = table.Rows.Count - 1; i >= 0; i--)
            //{
            //    foreach (DataColumn column in table.Columns)
            //    {
            //        if (table.Rows[i][column.ColumnName] == DBNull.Value
            //            //||string.IsNullOrWhiteSpace( table.Rows[i][column.ColumnName].ToString())
            //            )
            //        {
            //            rowDel = true;
            //            Errorlist.ColumnName = "Column Name: " + column.ColumnName + " and Row No: " + i.ToString();
            //            Errorlist.Message = " Has Empty/Blank Data";

            //        }
            //        else
            //        {
            //            rowDel = false;
            //        }
            //    }

            //    if (rowDel)
            //    {
            //        table.Rows[i].Delete();
            //        Errorlist.ColumnName =  "Row Number : " + i.ToString();
            //        Errorlist.Message = " Deleted";
            //    }
            //}
            //table.AcceptChanges();
            return table;
        }



        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (cmbImportType.Text != CONST_DATABASE)
            {
                _isDeleteTemp = true;
                btnImport.Enabled = true;
                LoadDataGrid();
            }
            else
            {
                try
                {
                    CommonDAL _cDal = new CommonDAL();
                    selectedTable = cmbTable.Text;
                    selectedType = cmbImportType.Text;
                    string[] cFields = new string[] { "IsVATComplete", "ID" };
                    string[] cVals = new string[] { "N", txtId.Text };
                    switch (selectedTable)
                    {
                        case CONST_SALETYPE:
                            dtTableResult = _cDal.GenericSelection("Sales", CONST_DBNAME, cFields, cVals, connVM);
                            loadedTable = CONST_SALETYPE;
                            break;
                        case CONST_PURCHASETYPE:
                            dtTableResult = _cDal.GenericSelection("Purchases", CONST_DBNAME, cFields, cVals, connVM);
                            //dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_PURCHASETYPE;
                            break;
                        case CONST_ISSUETYPE:
                            dtTableResult = _cDal.GenericSelection("Issues", CONST_DBNAME, cFields, cVals, connVM);
                            //dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_ISSUETYPE;
                            break;
                        case CONST_RECEIVETYPE:
                            dtTableResult = _cDal.GenericSelection("Receives", CONST_DBNAME, cFields, cVals, connVM);
                            //dgvLoadedTable.DataSource = dtTableResult;
                            loadedTable = CONST_RECEIVETYPE;
                            break;
                        case CONST_TRANSFERTYPE:
                            LoadFG();
                            break;
                        default:
                            break;
                    }
                    //dtTableResult.Columns.Remove("IsVATComplete");

                    OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);
                    if (errormessage.Count > 0)
                    {
                        FormErrorMessage.ShowDetails(errormessage);
                    }
                    errormessage.Clear();
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
            CommonDAL commonDal = new CommonDAL();

            string value = commonDal.settingValue("CompanyCode", "Code");

            ImportDAL dal = new ImportDAL();
            TransferIssueDAL transferIssueDal = new TransferIssueDAL();
            BranchProfileDAL branchDAL = new BranchProfileDAL();
            var importDal = new ImportDAL();

            var dt = branchDAL.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);

            var branchCode = dt.Rows[0]["IntegrationCode"].ToString();


            if (value == "SQR")
            {
                string transferExist = commonDal.settingValue("Import", "TransferExistContinue", connVM);

                if (transferExist == "N")
                {

                    var issueVms = transferIssueDal.SelectAllList(0, new[] { "ti.ImportIDExcel" }, new[] { txtId.Text }, null, null, connVM);

                    if (issueVms != null && issueVms.Count > 0)
                    {
                        var issueVm = issueVms.FirstOrDefault();

                        if (issueVm != null)
                            MessageBox.Show(@"This Transfer Id is already in system with Transfer no - " +
                                            issueVm.TransferIssueNo);

                        return;
                    }

                }

                var ids = txtId.Text.Split(',');
                Integration = true;

                if (branchCode != "UEN")
                {
                    dtTableResult = dal.GetFGSQRDbData(ids, dt, txtId.Text);
                }
                else
                {
                    DataTable table = new DataTable();
                    DataTable temp = new DataTable();

                    foreach (var id in ids)
                    {
                        var prefix = id[0];


                        if (prefix == 'H')
                        {
                            temp = importDal.GetFGSQRDbDataSenora(new[] { id.TrimStart('H') }, dt, txtId.Text);
                        }
                        else if (prefix == 'S')
                        {
                            temp = importDal.GetFGSQRDbDataSoap(new[] { id.TrimStart('S') }, dt, txtId.Text);
                        }


                        table.Merge(temp);
                    }

                    dtTableResult = table.Copy();
                }
            }

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

                    //fdlg.Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (*.xlsm)|*.xlsm|Excel(97-2003) files (*.xls)|*.xls|Text files (*.txt)|*.txt";
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
            if (loadedTable == "" || loadedTable == null)
            {
                return;
            }

            if (!IsRowSelected())
            {
                MessageBox.Show(this, "Please select at least one row!");
                return;
            }
            switch (loadedTable)
            {

                case CONST_PURCHASETYPE:
                    this.progressBar1.Visible = true;
                    backgroundWorkerPurchaseSave.RunWorkerAsync();
                    break;
                case CONST_ISSUETYPE:
                    this.progressBar1.Visible = true;
                    bgwIssueSave.RunWorkerAsync();
                    break;
                case CONST_RECEIVETYPE:
                    this.progressBar1.Visible = true;
                    bgwReceiveSave.RunWorkerAsync();
                    break;
                case CONST_VDSTYPE:
                    this.progressBar1.Visible = true;
                    bgwVDSSave.RunWorkerAsync();
                    break;
                case CONST_BOMTYPE:
                    this.progressBar1.Visible = true;
                    bgwBOMSave.RunWorkerAsync();
                    break;
                case CONST_TRANSFERTYPE:

                    SaveFGOut();

                    break;
                case CONST_TransferReceive:
                    this.progressBar1.Visible = true;
                    bgwTransferReceive.RunWorkerAsync();
                    break;
                default:
                    break;
            }
        }

        private void SaveFGOut()
        {

            SaveExcel_Audit();

            #region Square DataLoad

            CommonDAL dal = new CommonDAL();

            string value = dal.settingValue("CompanyCode", "Code", connVM);
            DataTable salesData = null;

            if (value == "SQR")
            {
                var filterdData = new DataTable();


                foreach (DataColumn column in dtTableResult.Columns)
                {
                    filterdData.Columns.Add(column.ColumnName);
                }


                foreach (DataRow row in dtTableResult.Rows)
                {

                    var currentRows = filterdData.Select("ProductCode= '" + row["ProductCode"] + "'");

                    if (currentRows.Length == 0)
                    {
                        var rows = dtTableResult.Select("ProductCode= '" + row["ProductCode"] + "'");

                        decimal quantity = 0;
                        foreach (DataRow dataRow in rows)
                        {
                            quantity += Convert.ToDecimal(dataRow["Quantity"]);
                        }

                        row["Quantity"] = quantity;

                        var items = row.ItemArray;

                        filterdData.Rows.Add(items);
                    }

                }

                salesData = filterdData;

                var pDal = new ProductDAL();

                var dateTime = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                foreach (DataRow row in salesData.Rows)
                {
                    var vms = pDal.SelectAll("0", new[] { "ProductCode" }, new[] { row["ProductCode"].ToString() });

                    if (vms != null && vms.Any())
                    {
                        var vm = vms.FirstOrDefault();

                        row["CostPrice"] = vm.NBRPrice;
                        row["VAT_Rate"] = vm.VATRate;
                        row["UOM"] = vm.UOM;
                    }

                    Integration = true;
                    row["TransferToBranchCode"] = cmbTransferTo.Text;
                    row["BranchCode"] = Program.BranchCode;
                    row["TransactionType"] = transactionType;
                    row["TransactionDateTime"] = dateTime;
                }

                dtTableResult = salesData.Copy();

            }



            #endregion

            this.progressBar1.Visible = true;
            bgwTransferIssue.RunWorkerAsync();
        }

        private void SaveExcel_Audit()
        {

            if (dtTableResult != null && dtTableResult.Rows.Count != 0)
            {

                string sourceFile = Program.ImportFileName;

                OrdinaryVATDesktop.SaveExcel_Audit(sourceFile);

            }

        }

        private bool IsRowSelected()
        {
            DataGridView gd1 = dgvLoadedTable;
            DataTable dt1 = (DataTable)gd1.DataSource;

            dtTableResult = new DataTable();

            string singleImport = new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "BOM", null, connVM);
            singleImport = "Y";

            if (singleImport == "Y" && preSelectTable == "BOM")
            {
                dtTableResult = dt1.Copy();
            }
            else
            {
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
            }

            return dtTableResult.Rows.Count > 0;
        }

        private void ImportBigData()
        {

            var saleDal = new SaleDAL();
            if (saleDal.GetUnProcessedCount() > 0 && _isDeleteTemp)
            {
                var dialogResult = MessageBox.Show(this,
                    @"There are still unprocessed data remains. All those data will be deleted.", "Are you sure?",
                    MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.Yes) return;
            }


            this.progressBar1.Visible = true;
            bgwBigData.RunWorkerAsync();

        }
        private void PurchaseImportBigData()
        {

            var purchaseDal = new PurchaseDAL();
            if (purchaseDal.GetUnProcessedCount() > 0)
            {
                var dialogResult = MessageBox.Show(this,
                    @"There are still unprocessed data remains. All those data will be deleted.", "Are you sure?",
                    MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.Yes) return;
            }


            this.progressBar1.Visible = true;
            bgwPurchaseBigData.RunWorkerAsync();

        }

        private void IssueImportBigData()
        {

            var issueDal = new IssueDAL();
            if (issueDal.GetUnProcessedCount() > 0)
            {
                var dialogResult = MessageBox.Show(this,
                    @"There are still unprocessed data remains. All those data will be deleted.", "Are you sure?",
                    MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.Yes) return;
            }


            this.progressBar1.Visible = true;
            bgwIssueBigData.RunWorkerAsync();

        }

        private void ReceiveImportBigData()
        {

            var saleDal = new SaleDAL();
            if (saleDal.GetUnProcessedCount_tempTable("ReceiveTempData") > 0)
            {
                var dialogResult = MessageBox.Show(this,
                    @"There are still unprocessed data remains. All those data will be deleted.", "Are you sure?",
                    MessageBoxButtons.YesNo);
                if (dialogResult != DialogResult.Yes) return;
            }


            this.progressBar1.Visible = true;
            bgwReceiveBigData.RunWorkerAsync();

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

        private void backgroundSaveSale_DoWork(object sender, DoWorkEventArgs e)
        {
            SaveExcel_Audit();
            SaveSale();
        }

        private void SaveSale()
        {
            #region try

            #region variables

            DataTable dtSaleM = new DataTable();
            DataTable dtSaleD = new DataTable();
            DataTable dtSaleE = new DataTable();
            bool CommercialImporter = false;
            decimal cTotalValue = 0;
            decimal cATVablePrice = 0;
            decimal cATVAmount = 0;
            decimal cWareHouseRent = 0;
            decimal cWareHouseVAT = 0;
            decimal cVATablePrice = 0;
            decimal cATVRate = 0;

            #endregion

            try
            {
                #region Excel and Db

                if (selectedType != CONST_TEXT)
                {
                    dtSaleM = new System.Data.DataTable();
                    string SingleSaleImport = new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "SaleImport");
                    SingleSaleImport = "Y";
                    if (SingleSaleImport.ToLower() == "y" || selectedType == CONST_DATABASE)
                    {
                        DataView view = new DataView(dtTableResult);
                        try
                        {
                            dtSaleM = view.ToTable(true, "ID", "Customer_Name", "Customer_Code", "Delivery_Address",
                                "Branch_Code", "Vehicle_No",
                                "Invoice_Date_Time", "Delivery_Date_Time", "Reference_No", "Comments", "Sale_Type",
                                "Previous_Invoice_No", "Is_Print", "Tender_Id",
                                "Post", "LC_Number", "Currency_Code", "CurrencyID", "CustomerID", "BranchId");
                            dtSaleD = new DataTable();
                            dtSaleD = view.ToTable(true, "ID", "Item_Code", "Item_Name", "Quantity", "NBR_Price", "UOM",
                                "VAT_Rate",
                                "SD_Rate", "Non_Stock", "Trading_MarkUp", "Type", "Discount_Amount", "Promotional_Quantity",
                                "VAT_Name", "Total_Price", "CommentsD", "ItemNo");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        if (ds == null)
                        {
                            MessageBox.Show(this, "There is no data to save!", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }

                        dtSaleM = dtTableResult.Copy();
                        dtSaleD = ds.Tables["SaleD"].Copy();
                        if (dtSaleD == null)
                        {
                            MessageBox.Show("Single sheet import is set to off!", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }

                        dtSaleE = ds.Tables["dtSaleE"];
                    }

                    #region Process DataTable

                    dtSaleM = OrdinaryVATDesktop.DtColumnAdd(dtSaleM, "Transection_Type", transactionType, "string");
                    dtSaleM = OrdinaryVATDesktop.DtColumnAdd(dtSaleM, "Created_By", Program.CurrentUser, "string");
                    dtSaleM = OrdinaryVATDesktop.DtColumnAdd(dtSaleM, "LastModified_By", Program.CurrentUser, "string");

                    dtSaleD = OrdinaryVATDesktop.DtColumnAdd(dtSaleD, "TotalValue", cTotalValue.ToString(), "string");

                    //dtSaleD.Columns.Add("TotalValue");
                    dtSaleD.Columns.Add("WareHouseRent");
                    dtSaleD.Columns.Add("WareHouseVAT");
                    dtSaleD.Columns.Add("ATVRate");
                    dtSaleD.Columns.Add("ATVablePrice");
                    dtSaleD.Columns.Add("ATVAmount");
                    dtSaleD.Columns.Add("IsCommercialImporter");
                    for (int i = 0; i < dtSaleD.Rows.Count; i++)
                    {
                        //dtSaleD.Rows[i]["TotalValue"] = cTotalValue;
                        dtSaleD.Rows[i]["WareHouseRent"] = cWareHouseRent;
                        dtSaleD.Rows[i]["WareHouseVAT"] = cWareHouseVAT;
                        dtSaleD.Rows[i]["ATVRate"] = cATVRate;
                        dtSaleD.Rows[i]["ATVablePrice"] = cATVablePrice;
                        dtSaleD.Rows[i]["ATVAmount"] = cATVAmount;
                        dtSaleD.Rows[i]["IsCommercialImporter"] = (CommercialImporter == true ? "Y" : "N").ToString();

                        if (CommercialImporter)
                        {
                            dtSaleD.Rows[i]["NBR_Price"] = Convert
                                .ToDecimal(Convert.ToDecimal(dtSaleD.Rows[i]["Total_Price"].ToString()) /
                                           Convert.ToDecimal(dtSaleD.Rows[i]["Quantity"].ToString()))
                                .ToString(); // CommercialImporterCalculation(dtSaleD.Rows[i]["Total_Price"].ToString(), dtSaleD.Rows[i]["VAT_Rate"].ToString(), dtSaleD.Rows[i]["Quantity"].ToString()).ToString();
                            dtSaleD.Rows[i]["TotalValue"] = cTotalValue;
                            dtSaleD.Rows[i]["WareHouseRent"] = cWareHouseRent;
                            dtSaleD.Rows[i]["WareHouseVAT"] = cWareHouseVAT;
                            dtSaleD.Rows[i]["ATVRate"] = cATVRate;
                            dtSaleD.Rows[i]["ATVablePrice"] = cATVablePrice;
                            dtSaleD.Rows[i]["ATVAmount"] = cATVAmount;
                            dtSaleD.Rows[i]["IsCommercialImporter"] = (CommercialImporter == true ? "Y" : "N").ToString();
                            dtSaleD.Rows[i]["NBR_Price"] = cVATablePrice.ToString();
                        }
                    }

                    #endregion
                }

                #endregion

                #region Text

                else
                {
                    dtSaleM = dtTableResult.Copy();
                    dtSaleD = ds.Tables[1];
                    foreach (DataRow dr in dtSaleD.Rows)
                    {
                        dr["VAT_Name"] = "VAT 4.3";
                    }

                    #region Detail Adjustment

                    for (int i = 0; i < dtSaleD.Rows.Count; i++)
                    {
                        dtSaleD.Rows[i]["TotalValue"] = cTotalValue;
                        dtSaleD.Rows[i]["WareHouseRent"] = cWareHouseRent;
                        dtSaleD.Rows[i]["WareHouseVAT"] = cWareHouseVAT;
                        dtSaleD.Rows[i]["ATVRate"] = cATVRate;
                        dtSaleD.Rows[i]["ATVablePrice"] = cATVablePrice;
                        dtSaleD.Rows[i]["ATVAmount"] = cATVAmount;
                        dtSaleD.Rows[i]["IsCommercialImporter"] = (CommercialImporter == true ? "Y" : "N").ToString();

                        if (CommercialImporter)
                        {
                            dtSaleD.Rows[i]["NBR_Price"] = Convert
                                .ToDecimal(Convert.ToDecimal(dtSaleD.Rows[i]["Total_Price"].ToString()) /
                                           Convert.ToDecimal(dtSaleD.Rows[i]["Quantity"].ToString()))
                                .ToString(); // CommercialImporterCalculation(dtSaleD.Rows[i]["Total_Price"].ToString(), dtSaleD.Rows[i]["VAT_Rate"].ToString(), dtSaleD.Rows[i]["Quantity"].ToString()).ToString();
                            dtSaleD.Rows[i]["TotalValue"] = cTotalValue;
                            dtSaleD.Rows[i]["WareHouseRent"] = cWareHouseRent;
                            dtSaleD.Rows[i]["WareHouseVAT"] = cWareHouseVAT;
                            dtSaleD.Rows[i]["ATVRate"] = cATVRate;
                            dtSaleD.Rows[i]["ATVablePrice"] = cATVablePrice;
                            dtSaleD.Rows[i]["ATVAmount"] = cATVAmount;
                            dtSaleD.Rows[i]["IsCommercialImporter"] = (CommercialImporter == true ? "Y" : "N").ToString();
                            dtSaleD.Rows[i]["NBR_Price"] = cVATablePrice.ToString();
                        }
                    }

                    #endregion
                }

                #endregion

                SaleDAL saleDal = new SaleDAL();
                sqlResults = saleDal.ImportData(dtSaleM, dtSaleD, dtSaleE, CommercialImporter, Program.BranchId, connVM, Program.CurrentUserID);
            }

            #endregion

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void backgroundSaveSale_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ShowingMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void ShowingMessage()
        {
            if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
            {
                if (cmbImportType.Text == CONST_DATABASE)
                {
                    //try
                    //{
                    //    var result = new CommonDAL().UpdateIsVATComplete(loadedTable, null, null);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(this, ex.Message);
                    //}
                }
                MessageBox.Show(this, "Import completed successfully");
            }
            else
            {
                MessageBox.Show(sqlResults[1], "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backgroundWorkerPurchaseSave_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                SaveExcel_Audit();

                PurchaseDAL puchaseDal = new PurchaseDAL();

                var count = dtTableResult
                    .AsEnumerable()
                    .Select(x => x.Field<string>("ID"))
                    .Distinct().Count() + 1;

                if (InvokeRequired)
                    Invoke((MethodInvoker)delegate { PercentBar(count); });

                BulkCallBack();
                sqlResults = puchaseDal.SaveTempPurchase(dtTableResult, Program.BranchCode, transactionType, Program.CurrentUser,
                    Program.BranchId, BulkCallBack, null, null, connVM);
                #region Split Method



                //IntegrationParam param = new IntegrationParam
                //{
                //    Data = dtTableResult,
                //    callBack = BulkCallBack,
                //    SetSteps = SetSteps,
                //    DefaultBranchId = Program.BranchId,
                //    TransactionType = transactionType,
                //    CurrentUser = Program.CurrentUser
                //};

                //sqlResults = puchaseDal.SavePurchase_Split(param);
                #endregion
                // SavePurchase();
            }
            catch (Exception exception)
            {
                sqlResults[0] = "fail";
                sqlResults[1] = exception.Message;
                FileLogger.Log("FormMasterImport", "backgroundWorkerPurchaseSave", exception.Message);
            }
        }

        void SetSteps(int steps)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { PercentBar(steps); });
        }

        private void SavePurchase()
        {
            #region try

            #region variables

            DataTable dtPurchaseM = new DataTable();
            DataTable dtPurchaseD = new DataTable();
            DataTable dtPurchaseI = new DataTable();
            DataTable dtPurchaseT = new DataTable();

            #endregion

            try
            {
                #region Excel and Db

                if (selectedType != CONST_TEXT)
                {
                    dtPurchaseM = new System.Data.DataTable();
                    string SingleImport = "Y"; //new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "PurchaseImport",null,connVM);
                    if (SingleImport.ToLower() == "y" || selectedType == CONST_DATABASE)
                    {
                        if (!dtTableResult.Columns.Contains("BranchCode"))
                        {
                            var BranchCode = new DataColumn("BranchCode") { DefaultValue = Program.BranchCode };
                            dtTableResult.Columns.Add(BranchCode);
                        }

                        DataView view = new DataView(dtTableResult);
                        try
                        {
                            dtPurchaseM = view.ToTable(true, "ID", "Vendor_Name", "Vendor_Code", "Referance_No", "LC_No",
                                "BE_Number", "Invoice_Date", "Receive_Date", "Post", "With_VDS", "Previous_Purchase_No",
                                "Comments", "Custom_House", "BranchCode");
                            dtPurchaseI = view.ToTable(true, "ID", "Item_Code", "Item_Name", "CnF_Amount", "Insurance_Amount",
                                "Assessable_Value", "CD_Amount", "RD_Amount", "TVB_Amount", "SD_Amount", "VAT_Amount",
                                "TVA_Amount", "ATV_Amount", "Others_Amount", "Remarks");
                            dtPurchaseD = view.ToTable(true, "ID", "Item_Code", "Item_Name", "Quantity", "Total_Price", "UOM",
                                "Type", "Rebate_Rate", "SD_Amount", "VAT_Amount");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        if (ds == null)
                        {
                            MessageBox.Show(this, "There is no data to save!", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }

                        dtPurchaseM = dtTableResult.Copy();
                        dtPurchaseD = ds.Tables["PurchaseD"].Copy();
                        dtPurchaseI = ds.Tables["PurchaseI"].Copy();
                        string TrackingTrace = new CommonDAL().settingsDesktop("TrackingTrace", "Tracking", null, connVM);
                        if (TrackingTrace == "Y")
                        {
                            dtPurchaseT = ds.Tables["PurchaseT"];
                        }

                        if (dtPurchaseD == null)
                        {
                            MessageBox.Show(this, "Single import setting is set to off!", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    dtPurchaseM = OrdinaryVATDesktop.DtColumnAdd(dtPurchaseM, "Transection_Type", transactionType, "string");
                    dtPurchaseM = OrdinaryVATDesktop.DtColumnAdd(dtPurchaseM, "Created_By", Program.CurrentUser, "string");
                    dtPurchaseM = OrdinaryVATDesktop.DtColumnAdd(dtPurchaseM, "LastModified_By", Program.CurrentUser, "string");
                }

                #endregion

                #region Text

                else
                {
                    dtPurchaseM = dtTableResult.Copy();
                    dtPurchaseD = ds.Tables[1];
                }

                #endregion

                PurchaseDAL puchaseDal = new PurchaseDAL();

                dtPurchaseM = OrdinaryVATDesktop.DtDateCheck(dtPurchaseM, new string[] { "Invoice_Date", "Receive_Date" });
                //dtPurchaseD = OrdinaryVATDesktop.DtDateCheck(dtSaleD, new string[] { "" });
                sqlResults = puchaseDal.ImportData(dtPurchaseM, dtPurchaseD, dtPurchaseI, dtPurchaseT, Program.BranchId, null, null, null, connVM, Program.CurrentUserID);
            }

            #endregion

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void backgroundWorkerPurchaseSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {

                    MessageBox.Show(this, "Import Completed successfully");
                }
                else
                {
                    MessageBox.Show(sqlResults[1], "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
                this.progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }

        private void IssueTableValidation(DataTable dt)
        {
            try
            {
                #region required column
                if (!dt.Columns.Contains("ID"))
                {
                    throw new Exception("ID column is missing ...");
                }
                else if (!dt.Columns.Contains("Issue_DateTime"))
                {
                    throw new Exception("Issue_DateTime column is missing ...");
                }
                else if (!dt.Columns.Contains("Item_Code"))
                {
                    throw new Exception("Item_Code column is missing ...");
                }
                else if (!dt.Columns.Contains("Item_Name"))
                {
                    throw new Exception("Item_Name column is missing ...");
                }
                else if (!dt.Columns.Contains("Quantity"))
                {
                    throw new Exception("Quantity column is missing ...");
                }
                else if (!dt.Columns.Contains("UOM"))
                {
                    throw new Exception("UOM column is missing ...");
                }
                #endregion

                #region Optional fields

                if (!dt.Columns.Contains("Comments"))
                {
                    dt.Columns.Add("Comments");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Comments"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Reference_No"))
                {
                    dt.Columns.Add("Reference_No");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Reference_No"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Return_Id"))
                {
                    dt.Columns.Add("Return_Id");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Return_Id"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Post"))
                {
                    dt.Columns.Add("Post");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Post"] = "-";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void SaleTableValidation(DataTable dt)
        {
            try
            {
                #region required column
                if (!dt.Columns.Contains("ID"))
                {
                    throw new Exception("ID column is missing ...");
                }
                else if (!dt.Columns.Contains("Customer_Name"))
                {
                    throw new Exception("Customer_Name column is missing ...");
                }
                else if (!dt.Columns.Contains("Customer_Code"))
                {
                    throw new Exception("Customer_Code column is missing ...");
                }
                else if (!dt.Columns.Contains("Delivery_Address"))
                {
                    throw new Exception("Delivery_Address column is missing ...");
                }
                else if (!dt.Columns.Contains("Invoice_Date_Time"))
                {
                    throw new Exception("Invoice_Date_Time column is missing ...");
                }
                else if (!dt.Columns.Contains("UOM"))
                {
                    throw new Exception("UOM column is missing ...");
                }
                else if (!dt.Columns.Contains("Post"))
                {
                    throw new Exception("Post column is missing ...");
                }
                else if (!dt.Columns.Contains("Item_Code"))
                {
                    throw new Exception("Item_Code column is missing ...");
                }
                else if (!dt.Columns.Contains("Item_Name"))
                {
                    throw new Exception("Item_Name column is missing ...");
                }
                else if (!dt.Columns.Contains("Quantity"))
                {
                    throw new Exception("Quantity column is missing ...");
                }
                else if (!dt.Columns.Contains("NBR_Price"))
                {
                    throw new Exception("NBR_Price column is missing ...");
                }
                else if (!dt.Columns.Contains("SD_Rate"))
                {
                    throw new Exception("SD_Rate column is missing ...");
                }
                else if (!dt.Columns.Contains("Total_Price"))
                {
                    throw new Exception("Total_Price column is missing ...");
                }


                #endregion

                #region Optional fields

                if (!dt.Columns.Contains("Vehicle_No"))
                {
                    dt.Columns.Add("Vehicle_No");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Vehicle_No"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Delivery_Date_Time"))
                {
                    dt.Columns.Add("Delivery_Date_Time");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Delivery_Date_Time"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Reference_No"))
                {
                    dt.Columns.Add("Reference_No");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Reference_No"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Comments"))
                {
                    dt.Columns.Add("Comments");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Comments"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Sale_Type"))
                {
                    dt.Columns.Add("Sale_Type");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Sale_Type"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Previous_Invoice_No"))
                {
                    dt.Columns.Add("Previous_Invoice_No");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Previous_Invoice_No"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Is_Print"))
                {
                    dt.Columns.Add("Is_Print");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Is_Print"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Tender_Id"))
                {
                    dt.Columns.Add("Tender_Id");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Tender_Id"] = "-";
                    }
                }
                if (!dt.Columns.Contains("LC_Number"))
                {
                    dt.Columns.Add("LC_Number");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["LC_Number"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Currency_Code"))
                {
                    dt.Columns.Add("Currency_Code");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Currency_Code"] = "-";
                    }
                }
                if (!dt.Columns.Contains("VAT_Rate"))
                {
                    dt.Columns.Add("VAT_Rate");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["VAT_Rate"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Non_Stock"))
                {
                    dt.Columns.Add("Non_Stock");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Non_Stock"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Trading_MarkUp"))
                {
                    dt.Columns.Add("Trading_MarkUp");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Trading_MarkUp"] = "0";
                    }
                }
                if (!dt.Columns.Contains("Type"))
                {
                    dt.Columns.Add("Type");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Type"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Discount_Amount"))
                {
                    dt.Columns.Add("Discount_Amount");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Discount_Amount"] = "0";
                    }
                }
                if (!dt.Columns.Contains("Promotional_Quantity"))
                {
                    dt.Columns.Add("Promotional_Quantity");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Promotional_Quantity"] = "0";
                    }
                }
                if (!dt.Columns.Contains("VAT_Name"))
                {
                    dt.Columns.Add("VAT_Name");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["VAT_Name"] = "-";
                    }
                }


                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void PurchaseTableValidation(DataTable dt)
        {
            try
            {
                #region required column
                if (!dt.Columns.Contains("ID"))
                {
                    throw new Exception("ID column is missing ...");
                }
                else if (!dt.Columns.Contains("Vendor_Name"))
                {
                    throw new Exception("Vendor_Name column is missing ...");
                }
                else if (!dt.Columns.Contains("Vendor_Code"))
                {
                    throw new Exception("Vendor_Code column is missing ...");
                }
                else if (!dt.Columns.Contains("Invoice_Date"))
                {
                    throw new Exception("Invoice_Date column is missing ...");
                }
                else if (!dt.Columns.Contains("Receive_Date"))
                {
                    throw new Exception("Receive_Date column is missing ...");
                }
                else if (!dt.Columns.Contains("Item_Code"))
                {
                    throw new Exception("Item_Code column is missing ...");
                }
                else if (!dt.Columns.Contains("Item_Name"))
                {
                    throw new Exception("Item_Name column is missing ...");
                }
                else if (!dt.Columns.Contains("Quantity"))
                {
                    throw new Exception("Quantity column is missing ...");
                }
                else if (!dt.Columns.Contains("Total_Price"))
                {
                    throw new Exception("Total_Price column is missing ...");
                }
                else if (!dt.Columns.Contains("UOM"))
                {
                    throw new Exception("UOM column is missing ...");
                }
                else if (!dt.Columns.Contains("VAT_Amount"))
                {
                    throw new Exception("VAT_Amount column is missing ...");
                }
                #endregion

                #region Optional fields

                if (!dt.Columns.Contains("Referance_No"))
                {
                    dt.Columns.Add("Referance_No");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Referance_No"] = "-";
                    }
                }
                if (!dt.Columns.Contains("LC_No"))
                {
                    dt.Columns.Add("LC_No");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["LC_No"] = "-";
                    }
                }
                if (!dt.Columns.Contains("BE_Number"))
                {
                    dt.Columns.Add("BE_Number");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["BE_Number"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Post"))
                {
                    dt.Columns.Add("Post");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Post"] = "N";
                    }
                }
                if (!dt.Columns.Contains("With_VDS"))
                {
                    dt.Columns.Add("With_VDS");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["With_VDS"] = "N";
                    }
                }
                if (!dt.Columns.Contains("Previous_Purchase_No"))
                {
                    dt.Columns.Add("Previous_Purchase_No");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Post"] = "0";
                    }
                }
                if (!dt.Columns.Contains("Comments"))
                {
                    dt.Columns.Add("Comments");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Post"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Custom_House"))
                {
                    dt.Columns.Add("Custom_House");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Custom_House"] = "-";
                    }
                }
                if (!dt.Columns.Contains("Type"))
                {
                    dt.Columns.Add("Type");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Type"] = "VAT";
                    }
                }
                if (!dt.Columns.Contains("Rebate_Rate"))
                {
                    dt.Columns.Add("Rebate_Rate");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Rebate_Rate"] = "0";
                    }
                }
                if (!dt.Columns.Contains("SD_Amount"))
                {
                    dt.Columns.Add("SD_Amount");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["SD_Amount"] = "0";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void bgwIssueSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaveExcel_Audit();

                IssueDAL issueDal = new IssueDAL();

                var count = dtTableResult
                    .AsEnumerable()
                    .Select(x => x.Field<string>("ID"))
                    .Distinct().Count();

                if (InvokeRequired)
                    Invoke((MethodInvoker)delegate { PercentBar(count); });


                sqlResults = issueDal.SaveTempIssue(dtTableResult, transactionType, Program.CurrentUser, Program.BranchId, BulkCallBack, null, null, connVM);

                //SaveIssue();
            }
            catch (Exception exception)
            {
                sqlResults[0] = "fail";
                sqlResults[1] = exception.Message;
                FileLogger.Log("FormMasterImport", "bgwIssueSave", exception.Message);

            }
        }

        private void SaveIssue()
        {
            #region try

            #region variables

            DataTable dtIssueM = new DataTable();
            DataTable dtIssueD = new DataTable();

            #endregion

            try
            {
                #region Excel and Db

                if (selectedType != CONST_TEXT)
                {
                    dtIssueM = new System.Data.DataTable();
                    string SingleImport = "Y"; //new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "IssueImport");

                    if (SingleImport.ToLower() == "y" || selectedType == CONST_DATABASE)
                    {
                        IssueTableValidation(dtTableResult);

                        if (!dtTableResult.Columns.Contains("Branch_Code"))
                        {
                            var column = new DataColumn("Branch_Code") { DefaultValue = Program.BranchCode };
                            dtTableResult.Columns.Add(column);
                        }

                        DataView view = new DataView(dtTableResult);
                        try
                        {
                            dtIssueM = view.ToTable(true, "ID", "Issue_DateTime", "Reference_No", "Comments", "Return_Id",
                                "Post", "Branch_Code");
                            dtIssueD = view.ToTable(true, "ID", "Item_Code", "Item_Name", "Quantity", "UOM");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        if (ds == null)
                        {
                            MessageBox.Show(this, "There is no data to save!", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }

                        dtIssueM = dtTableResult.Copy();
                        dtIssueD = ds.Tables["IssueD"].Copy();
                        if (dtIssueD == null)
                        {
                            MessageBox.Show(this, "Single import setting is set to off!", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    dtIssueM = OrdinaryVATDesktop.DtColumnAdd(dtIssueM, "Transection_Type", transactionType, "string");
                    dtIssueM = OrdinaryVATDesktop.DtColumnAdd(dtIssueM, "Created_By", Program.CurrentUser, "string");
                    dtIssueM = OrdinaryVATDesktop.DtColumnAdd(dtIssueM, "LastModified_By", Program.CurrentUser, "string");
                }

                #endregion

                #region Text

                else
                {
                    dtIssueM = dtTableResult.Copy();
                    dtIssueD = ds.Tables[1];
                }

                #endregion

                IssueDAL issueDal = new IssueDAL();

                dtIssueM = OrdinaryVATDesktop.DtDateCheck(dtIssueM, new string[] { "Issue_DateTime" });

                sqlResults = issueDal.ImportData(dtIssueM, dtIssueD, Program.BranchId, null, null, null, connVM);
            }

            #endregion

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bgwIssueSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ShowingMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
                this.progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }

        private void bgwReceiveSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaveExcel_Audit();
                ReceiveDAL receiveDal = new ReceiveDAL();

                //SetPercentBar();


                IntegrationParam param = new IntegrationParam
                {
                    Data = dtTableResult,
                    callBack = BulkCallBack,
                    SetSteps = SetSteps,
                    DefaultBranchId = Program.BranchId,
                    TransactionType = transactionType,
                    CurrentUser = Program.CurrentUser,
                    IsJoinReference = false
                };

                sqlResults = receiveDal.SaveReceive_Split(param);

                //sqlResults = receiveDal.SaveTempReceive(dtTableResult, transactionType, Program.CurrentUser, Program.BranchId, BulkCallBack);
            }
            catch (Exception exception)
            {
                sqlResults[0] = "fail";
                sqlResults[1] = exception.Message;
                FileLogger.Log("FormMasterImport", "bgwReceiveSave", exception.Message);

            }

            // SaveReceive();
        }

        private void SetPercentBar()
        {
            var count = dtTableResult
                .AsEnumerable()
                .Select(x => x.Field<string>("ID"))
                .Count();

            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { PercentBar(count); });
        }

        private void SaveReceive()
        {
            #region variables

            DataTable dtReceiveM = new DataTable();
            DataTable dtReceiveD = new DataTable();
            CommonImportDAL cImport = new CommonImportDAL();
            int txtTotalAmount = 0;

            #endregion

            try
            {
                #region Excel and Db

                if (selectedType != CONST_TEXT)
                {
                    dtReceiveM = new System.Data.DataTable();
                    string SingleImport = new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "PurchaseImport");
                    if (SingleImport.ToLower() == "y" || selectedType == CONST_DATABASE)
                    {
                        if (!dtTableResult.Columns.Contains("BranchCode"))
                        {
                            var branchCode = new DataColumn("BranchCode") { DefaultValue = Program.BranchCode };
                            dtTableResult.Columns.Add(branchCode);
                        }

                        DataView view = new DataView(dtTableResult);
                        try
                        {
                            dtReceiveM = view.ToTable(true, "ID", "Receive_DateTime", "Reference_No", "Comments", "Return_Id",
                                "Post", "CustomerCode", "BranchCode");
                            dtReceiveD = view.ToTable(true, "ID", "Item_Code", "Item_Name", "Quantity", "UOM", "NBR_Price",
                                "VAT_Name");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        if (ds == null)
                        {
                            MessageBox.Show(this, "There is no data to save!", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }

                        dtReceiveM = dtTableResult.Copy();
                        dtReceiveD = ds.Tables["ReceiveD"].Copy();
                        if (dtReceiveD == null)
                        {
                            MessageBox.Show(this, "Single import setting is set to off!", this.Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string IssueFromBOM = new CommonDAL().settingsDesktop("IssueFromBOM", "IssueFromBOM");
                    dtReceiveM = OrdinaryVATDesktop.DtColumnAdd(dtReceiveM, "Transection_Type", transactionType, "string");
                    dtReceiveM = OrdinaryVATDesktop.DtColumnAdd(dtReceiveM, "Created_By", Program.CurrentUser, "string");
                    dtReceiveM = OrdinaryVATDesktop.DtColumnAdd(dtReceiveM, "LastModified_By", Program.CurrentUser, "string");
                    dtReceiveM = OrdinaryVATDesktop.DtColumnAdd(dtReceiveM, "From_BOM", IssueFromBOM, "string");
                    dtReceiveM = OrdinaryVATDesktop.DtColumnAdd(dtReceiveM, "Total_VAT_Amount", "0", "string");
                    dtReceiveM =
                        OrdinaryVATDesktop.DtColumnAdd(dtReceiveM, "Total_Amount", txtTotalAmount.ToString(), "string");


                    dtReceiveD.Columns.Add("item_No");
                    //foreach (DataRow row in dtReceiveD.Rows)
                    //{
                    //    string itemNo = cImport.FindItemId(row["Item_Name"].ToString(), row["Item_Code"].ToString().Trim(), null, null).ToString();
                    //    row["item_No"] = itemNo.Trim();
                    //}
                }

                #endregion

                #region Text

                else
                {
                    dtReceiveM = dtTableResult.Copy();
                    dtReceiveD = ds.Tables[1];
                    dtReceiveD.Columns.Add("item_No");
                    //foreach (DataRow row in dtReceiveD.Rows)
                    //{
                    //    string itemNo = cImport.FindItemId("", row["Item_Code"].ToString().Trim(), null, null).ToString();
                    //    row["item_No"] = itemNo.Trim();
                    //}
                }

                #endregion

                ReceiveDAL receiveDal = new ReceiveDAL();

                dtReceiveM = OrdinaryVATDesktop.DtDateCheck(dtReceiveM, new string[] { "Receive_DateTime" });
                sqlResults = receiveDal.ImportData(dtReceiveM, dtReceiveD, Program.BranchId, null, null, null, connVM);
            }



            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bgwReceiveSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ShowingMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
                this.progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }

        private void bgwVDSSave_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            #region variables
            DataTable dtVDSM = new DataTable();
            DataTable dtVDSD = new DataTable();
            #endregion
            try
            {
                string singleImport = "Y"; //new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "VDSImport",null,connVM);

                if (singleImport.ToLower() == "n")
                {
                    if (ds != null)
                    {
                        dtVDSM = dtTableResult.Copy();
                        dtVDSD = ds.Tables["VDSD"].Copy();

                        DataView view = new DataView(dtVDSD);


                        string query = "";

                        foreach (DataRow dr in dtVDSM.Rows)
                        {
                            string val = dr["Id"].ToString() + ",";
                            query += val;
                        }

                        query = query.Trim(',');
                        view.RowFilter = "ID IN(" + query + ")";

                        dtVDSD = new DataTable();
                        dtVDSD = view.ToTable();

                        dtVDSM = OrdinaryVATDesktop.DtColumnAdd(dtVDSM, "Transection_Type", transactionType, "string");
                        dtVDSM = OrdinaryVATDesktop.DtColumnAdd(dtVDSM, "Created_By", Program.CurrentUser, "string");
                        dtVDSM = OrdinaryVATDesktop.DtColumnAdd(dtVDSM, "LastModified_By", Program.CurrentUser, "string");

                        dtVDSD = OrdinaryVATDesktop.DtDateCheck(dtVDSD, new string[] { "Bill_Date", "Issue_Date" });
                        DepositDAL depositDal = new DepositDAL();
                        sqlResults = depositDal.ImportData(dtVDSM, dtVDSD, Program.BranchId, connVM);
                    }
                }
                else
                {
                    var depositDAl = new DepositDAL();

                    DataTable dt = dtTableResult.Copy();

                    dt.Columns["Effect_Date"].ColumnName = "Deposit_Date";

                    if (transactionType == "SaleVDS")
                    {
                        dt.Columns["VDS_Certificate_No"].ColumnName = "Treasury_No";
                        dt.Columns["VDS_Certificate_Date"].ColumnName = "BankDepositDate";
                        dt.Columns["Tax_Deposit_Account_Code"].ColumnName = "Cheque_No";
                        dt.Columns["Tax_Deposit_Date"].ColumnName = "Cheque_Date";
                        dt.Columns["Tax_Deposit_Serial_No"].ColumnName = "Cheque_Bank";
                        dt.Columns["Customer_Name"].ColumnName = "Vendor_Name";
                        dt.Columns["Customer_Code"].ColumnName = "Vendor_Code";

                    }

                    sqlResults = depositDAl.SaveTempVDS(dt, Program.BranchCode, transactionType, Program.CurrentUser,
                        Program.BranchId, null, null, connVM);

                    #region Split
                    //IntegrationParam param = new IntegrationParam
                    //{
                    //    Data = dt,
                    //    BranchCode=Program.BranchCode,
                    //    TransactionType = transactionType,
                    //    CurrentUser = Program.CurrentUser,
                    //    DefaultBranchId = Program.BranchId,

                    //};

                    //sqlResults = depositDAl.SaveVDS_Split(param);
                    #endregion

                }
            #endregion
            }
            catch (Exception ex)
            {
                sqlResults[1] = ex.Message;
                sqlResults[0] = "fail";
                FileLogger.Log("FormMasterImport", "bgwVdsSave", ex.Message);

            }
        }

        private void bgwVDSSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {

                    MessageBox.Show(this, "Import Completed successfully");
                }
                else
                {
                    MessageBox.Show(sqlResults[1], "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void bgwBOMSave_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            #region variables
            DataTable dtProduct = new DataTable();
            DataTable dtOH = new DataTable();
            List<BOMItemVM> bomItems;
            BOMNBRVM bomNbrs;
            List<BOMOHVM> bomOhs;// = new List<BOMOHVM>();
            #endregion
            try
            {
                string singleImport = "Y"; //new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "BOM");               

                if (singleImport.ToLower() == "y")
                {
                    BOMDAL bomdal = new BOMDAL();

                    dtTableResult.Columns.Add(new DataColumn() { ColumnName = "BranchId", DefaultValue = Program.BranchId });

                    bool useQtyFlag = dtTableResult.Columns.Contains("UseQuantity");

                    foreach (DataRow row in dtTableResult.Rows)
                    {
                        row["WastageQuantity"] = Program.ParseDecimal(row["WastageQuantity"].ToString());

                        if (useQtyFlag)
                        {
                            row["UseQuantity"] = Program.ParseDecimal(row["UseQuantity"].ToString());
                        }

                        row["TotalQuantity"] = Program.ParseDecimal(row["TotalQuantity"].ToString());
                        row["Cost"] = Program.ParseDecimal(row["Cost"].ToString());
                    }


                    sqlResults = bomdal.ImportBOM(dtTableResult);

                }
                else
                {
                    if (ds != null)
                    {
                        dtProduct = dtTableResult.Copy();
                        dtOH = ds.Tables["InputOH"].Copy();

                        dtProduct = OrdinaryVATDesktop.DtDateCheck(dtProduct, new string[] { "FirstSupplyDate", "EffectDate" });
                        //dtOH = OrdinaryVATDesktop.DtDateCheck(dtOH, new string[] { "" });


                        #region Process model
                        BOMDAL bom1 = new BOMDAL();
                        string Vat_Name = "";
                        DateTime dateEffectDate = new DateTime();
                        ProductDAL productDal = new ProductDAL();
                        CustomerDAL customerDAL = new CustomerDAL();
                        int BOMDPlaceA = Convert.ToInt32(new CommonDAL().settingsDesktop(CONST_BOMTYPE, "Amount"));
                        int BOMDPlaceQ = Convert.ToInt32(new CommonDAL().settingsDesktop(CONST_BOMTYPE, "Quantity"));
                        string str1 = "";
                        bomNbrs = new BOMNBRVM();
                        for (int i = 0; i < dtProduct.Rows.Count; i++)
                        {
                            #region Finish
                            decimal vVateRate = 0;
                            decimal vSDRate = 0;
                            decimal vVatAmount = 0;
                            decimal vMarkupPercent = 0;
                            decimal vMarkupValue = 0;
                            decimal vPacketPrice = 0;
                            decimal vNbrPrice = 0;
                            decimal vAdditionalTotal = 0;
                            decimal vRawTotal = 0;
                            decimal vSDAmount = 0;
                            decimal vPackingTotal = 0;
                            decimal vRebateTotal = 0;
                            string productGroup = dtProduct.Rows[i]["Type"].ToString().Trim();
                            string pCode = dtProduct.Rows[i]["P-Code"].ToString().Trim();
                            string finishItemName = dtProduct.Rows[i]["FinishItemName"].ToString().Trim();
                            string packSize = dtProduct.Rows[i]["Pack size"].ToString().Trim();
                            string vatName = dtProduct.Rows[i]["VATName"].ToString().Trim();
                            string effecrDateTemp = dtProduct.Rows[i]["EffectDate"].ToString().Trim();
                            string nbrPrice = dtProduct.Rows[i]["VATABLE PRICE"].ToString().Trim();
                            nbrPrice = Program.FormatingNumeric(nbrPrice, BOMDPlaceA).ToString();
                            string packetPrice = dtProduct.Rows[i]["PacketPrice"].ToString().Trim();
                            string vatRae = dtProduct.Rows[i]["VATRate"].ToString().Trim();
                            string sdRae = dtProduct.Rows[i]["SDRate"].ToString().Trim();
                            string tradingMarkup = dtProduct.Rows[i]["TradingMarkup"].ToString().Trim();
                            string margin = dtProduct.Rows[i]["Margin"].ToString().Trim();
                            string comments = dtProduct.Rows[i]["Remarks"].ToString().Trim();
                            string CustomerCode = dtProduct.Rows[i]["CustomerCode"].ToString().Trim();

                            string finishItemNo = "";

                            Debug.WriteLine("Finish Name '" + finishItemName + "' and code '" + pCode + "'");

                            if (string.IsNullOrEmpty(productGroup))
                                throw new ArgumentNullException("ProductGroup", "Could not find product Type in Product Sheet for ('" + finishItemName + "'('" + pCode + "'))");
                            DataRow[] OHOverheads;
                            DataRow[] OHRaws;//= new DataRow[];//

                            #region fitemno
                            if (string.IsNullOrEmpty(pCode))
                            {
                                if (string.IsNullOrEmpty(finishItemName))
                                {
                                    throw new ArgumentNullException("ProductName", "Could not find product name('" + finishItemName + "'('" + pCode + "'))");
                                }
                                else
                                {
                                    finishItemNo = productDal.GetProductNoByGroup(finishItemName, productGroup, connVM);
                                    if (string.IsNullOrEmpty(finishItemNo))
                                    {
                                        throw new ArgumentNullException("ProductName", "Could not find product('" + finishItemName + "'('" + pCode + "')) in database");
                                    }
                                    else
                                    {
                                        OHRaws =
                                           dtOH.Select("Product_Name='" + finishItemName +
                                       "'and VATName= '" + vatName + "'  AND (InputType='Raw' OR InputType='Pack' or InputType='Overhead' or InputType = 'Trading' or InputType = 'WIP' )");

                                        OHOverheads = dtOH.Select("Product_Name='" + finishItemName + "' and VATName= '" + vatName + "' AND (InputType='Overhead')");

                                    }
                                }
                            }
                            else
                            {
                                finishItemNo = productDal.GetProductNoByGroup_Code(pCode, productGroup, connVM);//
                                if (string.IsNullOrEmpty(finishItemNo))
                                {
                                    if (string.IsNullOrEmpty(finishItemName))
                                    {
                                        throw new ArgumentNullException("ProductName", "Could not find product name('" + finishItemName + "'('" + pCode + "'))");
                                    }
                                    else
                                    {
                                        finishItemNo = productDal.GetProductNoByGroup(finishItemName, productGroup, connVM);
                                        if (string.IsNullOrEmpty(finishItemNo))
                                        {
                                            throw new ArgumentNullException("ProductName", "Could not find product('" + finishItemName + "'('" + pCode + "')) in database");
                                        }
                                        else
                                        {

                                            OHRaws =
                                           dtOH.Select("Product_Name='" + finishItemName +
                                       "' and VATName= '" + vatName + "'  AND (InputType='Raw' OR InputType='Pack' or InputType='Overhead'  or InputType = 'Trading' or InputType = 'WIP')");
                                            OHOverheads = dtOH.Select("Product_Name='" + finishItemName + "' and VATName= '" + vatName + "' AND (InputType='Overhead')");

                                        }
                                    }
                                }
                                else
                                {
                                    //Convert([Col Name],‘System.String’)
                                    OHRaws =
                                           dtOH.Select("Convert(Product_Code,'System.String')='" + pCode + "' " +
                                                       "and VATName= '" + vatName + "'  AND (InputType='Raw' OR InputType='Pack' or InputType='Overhead' or InputType = 'Trading' or InputType = 'WIP')");
                                    OHOverheads = dtOH.Select("Convert(Product_Code,'System.String')='" + pCode + "' and VATName= '" + vatName + "' AND (InputType='Overhead')");

                                }

                            }

                            //var oh1 = OHOverheads.Where(x => x["Material Name"].ToString().ToLower() == "gas").CopyToDataTable();

                            #endregion fitemno

                            if (OHRaws.Count() <= 0)
                            {
                                throw new ArgumentNullException("ProductName", "Could not find product(" + finishItemName + "(" + pCode + ") and VATName " + vatName + ") in database");
                            }

                            DataTable dt = new DataTable();
                            dt = customerDAL.SearchCustomerByCode(CustomerCode, connVM);
                            string CustomerID = "0";
                            if (dt.Rows.Count > 0)
                            {
                                CustomerID = dt.Rows[0]["CustomerID"].ToString();
                            }
                            bomNbrs.ItemNo = finishItemNo;
                            bomNbrs.FinishItemName = finishItemName;
                            bomNbrs.CustomerID = CustomerID;

                            if (!string.IsNullOrEmpty(vatName))
                            {
                                Vat_Name = vatName;
                            }
                            if (!string.IsNullOrEmpty(vatRae))
                            {
                                vVateRate = Convert.ToDecimal(vatRae);
                            }

                            if (!string.IsNullOrEmpty(nbrPrice))
                            {
                                vNbrPrice = Convert.ToDecimal(nbrPrice);

                            }
                            if (!string.IsNullOrEmpty(sdRae))
                            {
                                vSDRate = Convert.ToDecimal(sdRae);
                            }

                            if (!string.IsNullOrEmpty(tradingMarkup))
                            {
                                vMarkupPercent = Convert.ToDecimal(tradingMarkup);
                            }
                            if (string.IsNullOrEmpty(Vat_Name))
                            {
                                throw new ArgumentNullException("VATName", "Vat name is empty");
                            }
                            if (!string.IsNullOrEmpty(margin))
                                bomNbrs.VATName = Vat_Name;
                            if (!string.IsNullOrEmpty(nbrPrice))
                                bomNbrs.PNBRPrice = Convert.ToDecimal(nbrPrice); // vp


                            if (string.IsNullOrEmpty(effecrDateTemp))
                                throw new ArgumentNullException("VATName", "unable to process Effect date");
                            dateEffectDate = Convert.ToDateTime(effecrDateTemp);
                            bomNbrs.EffectDate = dateEffectDate.ToString("yyyy-MMM-dd");



                            bomNbrs.VATRate = vVateRate;

                            if (!string.IsNullOrEmpty(tradingMarkup))
                                bomNbrs.TradingMarkup = Convert.ToDecimal(tradingMarkup);

                            bomNbrs.Comments = comments;


                            bomNbrs.SDRate = vSDRate;
                            bomNbrs.UOM = packSize.Trim();
                            bomNbrs.BranchId = Program.BranchId;
                            #endregion Finish

                            #region Material Process Pack & Raw
                            DataRow[] inpType =
                               dtOH.Select("InputType <>'Raw' and InputType<>'Pack' and InputType<>'Overhead' and InputType<>'Trading' and InputType<>'Finish' and InputType <> 'WIP'");
                            if (inpType != null || !inpType.Any())
                            {
                                foreach (DataRow row in inpType)
                                {
                                    string materialName = row["Material Name"].ToString().Trim();
                                    string inputType = row["InputType"].ToString().Trim();

                                    throw new ArgumentNullException("Input Type", "Input Type ('" + inputType + "')  for Material Name ('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "')) not in database");

                                }
                                str1 = "";
                            }

                            str1 = "";
                            int counter = 1;
                            bomItems = new List<BOMItemVM>();

                            //DataTable test =  OHRaws.CopyToDataTable();


                            double wastageQty, ohUnitCose, totalQty, useQty, vRebatePercent, vSubTotal = 0;
                            foreach (DataRow row in OHRaws)
                            {
                                BOMItemVM bomItem = new BOMItemVM();
                                string materialItemNo = "";
                                string inputType = row["InputType"].ToString().Trim();
                                string ohPacksize = row["Packsize"].ToString().Trim();
                                string materialCode = row["Material_Code"].ToString().Trim();
                                string materialName = row["Material Name"].ToString().Trim();


                                string ohUOM = row["UOM"].ToString().Trim();

                                //Debug.WriteLine("Materials Name '" + materialName + "' and code '" + materialCode + "' Finish Name '" + finishItemName + "' and code '" + pCode + "'");
                                if (string.IsNullOrEmpty(inputType))
                                {
                                    throw new ArgumentNullException("InputType", "Unable to process input type value  of Product ('" + finishItemName + "'('" + pCode + "'))");
                                }

                                else if (inputType.ToLower() != "pack" && inputType.ToLower() != "overhead" && inputType.ToLower() != "finish" && inputType.ToLower() != "trading" && inputType.ToLower() != "raw" && inputType.ToLower() != "wip")
                                {
                                    throw new ArgumentNullException("Input Type", "Input Type ('" + inputType + "')  of Product ('" + finishItemName + "'('" + pCode + "')) not in database");

                                }
                                #region Find materialCode
                                if (string.IsNullOrEmpty(inputType))
                                    throw new ArgumentNullException("ProductGroup", "Could not find product Type in InputOH Sheet for ('" + materialName + "')  of Product ('" + finishItemName + "'('" + pCode + "'))");

                                if (string.IsNullOrEmpty(materialCode))
                                {
                                    if (string.IsNullOrEmpty(materialName))
                                    {
                                        throw new ArgumentNullException("ProductName", "Could not find product name('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "'))");
                                    }
                                    else
                                    {
                                        materialItemNo = productDal.GetProductNoByGroup(materialName, inputType, connVM);
                                        if (string.IsNullOrEmpty(materialItemNo))
                                        {
                                            throw new ArgumentNullException("ProductName", "Could not find product('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "')) in database");
                                        }
                                    }
                                }
                                else
                                {
                                    materialItemNo = productDal.GetProductNoByGroup_Code(materialCode, inputType, connVM);
                                    if (string.IsNullOrEmpty(materialItemNo))
                                    {
                                        if (string.IsNullOrEmpty(materialName))
                                        {
                                            throw new ArgumentNullException("ProductName", "Could not find product name('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "'))");
                                        }
                                        else
                                        {
                                            materialItemNo = productDal.GetProductNoByGroup(materialName, inputType, connVM);
                                            if (string.IsNullOrEmpty(materialItemNo))
                                            {
                                                throw new ArgumentNullException("ProductName", "Could not find product('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "')) in database");
                                            }
                                        }

                                    }

                                }
                                #endregion Find materialCode
                                var vtotalQty = row["TotalQty"].ToString().Trim();

                                if (string.IsNullOrEmpty(vtotalQty))
                                {
                                    totalQty = 0;
                                }
                                else
                                {
                                    vtotalQty = Program.FormatingNumeric(vtotalQty, BOMDPlaceQ).ToString();
                                    totalQty = Convert.ToDouble(vtotalQty);
                                }

                                var wqty = Program.ParseDecimal(row["WastageQty"].ToString());
                                //var wqty = Decimal.Parse(row["WastageQty"].ToString(), System.Globalization.NumberStyles.Float).ToString();
                                if (string.IsNullOrEmpty(wqty))
                                {
                                    wastageQty = 0;
                                }
                                else
                                {
                                    wqty = Program.FormatingNumeric(wqty, BOMDPlaceQ).ToString();
                                    wastageQty = Convert.ToDouble(wqty);
                                }

                                var vuseQty = totalQty - wastageQty;// row["UseQty"].ToString().Trim();

                                useQty = Convert.ToDouble(vuseQty);

                                var vRebate = row["Rebate%"].ToString().Trim();
                                if (string.IsNullOrEmpty(vRebate))
                                {
                                    vRebatePercent = 0;
                                }
                                else
                                {
                                    vRebatePercent = Convert.ToDouble(vRebate);
                                }

                                var vSTotal = row["SubTotal"].ToString().Trim();
                                if (string.IsNullOrEmpty(vSTotal))
                                {
                                    vSubTotal = 0;
                                }
                                else
                                {
                                    vSTotal = Program.FormatingNumeric(vSTotal, BOMDPlaceA).ToString();
                                    vSubTotal = Convert.ToDouble(vSTotal);
                                }

                                decimal vuomc = 0;
                                decimal vuomPrice = 0;
                                decimal vuomUseQty = 0;
                                decimal vuomWastageQty = 0;
                                string uomc = string.Empty;//= row["UOMc"].ToString().Trim();
                                string uomn = string.Empty;//= row["UOMn"].ToString().Trim();


                                #region uomn
                                if (inputType != "Overhead")
                                {
                                    if (string.IsNullOrEmpty(materialCode))
                                    {
                                        if (string.IsNullOrEmpty(materialName))
                                        {
                                            throw new ArgumentNullException("ProductName", "Could not find product name('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "'))");
                                        }
                                        else
                                        {

                                            uomn = productDal.GetProductUOMn(materialName, inputType, connVM);

                                            if (string.IsNullOrEmpty(uomn))
                                            {
                                                throw new ArgumentNullException("ProductName", "Could not find product('" + materialName + "')  of Product ('" + finishItemName + "'('" + pCode + "'))in database");
                                            }
                                        }
                                    }
                                    else
                                    {

                                        uomn = productDal.GetProductCodeUOMn(materialCode, inputType, connVM);

                                        if (string.IsNullOrEmpty(uomn))
                                        {
                                            if (string.IsNullOrEmpty(materialName))
                                            {
                                                throw new ArgumentNullException("ProductName", "Could not find product name('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "'))");
                                            }
                                            else
                                            {

                                                uomn = productDal.GetProductUOMn(materialName, inputType, connVM);
                                                if (string.IsNullOrEmpty(uomn))
                                                {
                                                    throw new ArgumentNullException("ProductName", "Could not find product('" + materialName + "') of Product ('" + finishItemName + "'('" + pCode + "')) in database");
                                                }
                                            }

                                        }

                                    }






                                #endregion uomn
                                    try
                                    {
                                        uomc = productDal.GetProductUOMc(uomn, ohUOM, connVM);

                                        if (uomn.ToLower() == ohUOM.ToLower())
                                        {
                                            uomc = "1";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                    if (!string.IsNullOrEmpty(uomc))
                                    {
                                        vuomc = Convert.ToDecimal(uomc);
                                    }
                                    else
                                    {
                                        throw new ArgumentNullException("ProductName", "Could not find product's( '" + materialName + "') UOM : ('" + uomn + "')  to : ('" + ohUOM + "')  of Product ('" + finishItemName + "'('" + pCode + "')) in database");
                                    }

                                }

                                vuomUseQty = Convert.ToDecimal(useQty) * vuomc;
                                vuomWastageQty = Convert.ToDecimal(wastageQty) * vuomc;

                                if (!string.IsNullOrEmpty(inputType))
                                {
                                    if (inputType != "Pack" && inputType != "Overhead")
                                    {
                                        vRawTotal = vRawTotal + Convert.ToDecimal(vSubTotal);
                                    }
                                    else if (inputType == "Pack")
                                    {
                                        vPackingTotal = vPackingTotal + Convert.ToDecimal(vSubTotal);
                                    }
                                    else if (inputType == "Overhead" && vRebatePercent > 0)
                                    {
                                        vRebateTotal = vRebateTotal + Convert.ToDecimal(vSubTotal);
                                    }
                                }
                                vMarkupValue = Convert.ToDecimal(vSubTotal) * vMarkupPercent / 100;
                                vSDAmount = (Convert.ToDecimal(vSubTotal) + vMarkupValue) * vSDRate / 100;
                                vVatAmount = (Convert.ToDecimal(vSubTotal) + vMarkupValue + vSDAmount) * vVateRate / 100;

                                bomItem.RawItemNo = materialItemNo;
                                bomItem.FinishItemNo = finishItemNo;
                                bomItem.RebateRate = Convert.ToDecimal(vRebatePercent);
                                if (bomItem.RawItemNo == "41")
                                {

                                }

                                bomItem.ActiveStatus = "Y";

                                bomItem.RawItemName = materialName;
                                if (inputType == "Overhead")
                                {
                                    if (vRebatePercent == 0)
                                    {
                                        bomItem.UseQuantity = 1;
                                        bomItem.WastageQuantity = 0;
                                        bomItem.TotalQuantity = 1;
                                        bomItem.UnitCost = 0;
                                        bomItem.UOM = "-";
                                        bomItem.UOMc = 1;
                                        bomItem.UOMn = "-";
                                        bomItem.UOMPrice = 0;
                                        bomItem.UOMUQty = 1;
                                        bomItem.UOMWQty = 0;
                                        bomItem.Cost = 0;
                                    }
                                    else
                                    {
                                        bomItem.UseQuantity = 1;
                                        bomItem.WastageQuantity = 0;
                                        bomItem.TotalQuantity = 1;
                                        bomItem.UnitCost = Convert.ToDecimal(vSubTotal);
                                        bomItem.UOM = "-";
                                        bomItem.UOMc = 1;
                                        bomItem.UOMn = "-";
                                        bomItem.UOMPrice = Convert.ToDecimal(vSubTotal);
                                        bomItem.UOMUQty = 1;
                                        bomItem.UOMWQty = 0;
                                        bomItem.Cost = Convert.ToDecimal(vSubTotal);
                                    }
                                }
                                else
                                {
                                    bomItem.UseQuantity = Convert.ToDecimal(useQty);
                                    if (!string.IsNullOrEmpty(wastageQty.ToString().Trim()))
                                        bomItem.WastageQuantity = Convert.ToDecimal(wastageQty);
                                    bomItem.TotalQuantity = vuomUseQty + vuomWastageQty;// Convert.ToDecimal(totalQty);
                                    var tt12 = vuomUseQty + vuomWastageQty;

                                    if (Convert.ToDecimal(tt12) <= 0)
                                        throw new ArgumentNullException("Quantity", "Could not find product's( '" + materialName + "') useQty and WastageQty  of Product ('" + finishItemName + "'('" + pCode + "')) in import sheet");
                                    vuomPrice = Convert.ToDecimal(vSubTotal) / ((vuomUseQty + vuomWastageQty));
                                    bomItem.UnitCost = Convert.ToDecimal(vSubTotal) / ((vuomUseQty + vuomWastageQty)) * vuomc;

                                    bomItem.UOM = ohUOM;
                                    bomItem.UOMc = vuomc;
                                    bomItem.UOMn = uomn;
                                    bomItem.UOMPrice = vuomPrice;
                                    bomItem.UOMUQty = vuomUseQty;
                                    bomItem.UOMWQty = vuomWastageQty;
                                    bomItem.Cost = Convert.ToDecimal(vSubTotal);
                                }
                                bomItem.CreatedBy = Program.CurrentUser;
                                bomItem.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                                bomItem.LastModifiedBy = Program.CurrentUser;
                                bomItem.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                                bomItem.VATRate = vVateRate;
                                bomItem.SD = vSDRate;
                                bomItem.EffectDate = dateEffectDate.ToString("yyyy-MMM-dd");
                                bomItem.BOMLineNo = "" + counter;
                                bomItem.PacketPrice = vPacketPrice;
                                bomItem.NBRPrice = vNbrPrice;
                                bomItem.TradingMarkUp = vMarkupValue;
                                bomItem.RawItemType = inputType;
                                bomItem.Post = "N";
                                bomItem.TradingMarkUp = vMarkupPercent;
                                bomItem.SDAmount = vSDAmount;
                                bomItem.MarkUpValue = vMarkupValue;
                                bomItem.VatAmount = vVatAmount;


                                bomItem.PBOMId = "0";
                                bomItem.BranchId = Program.BranchId;
                                bomItems.Add(bomItem);

                                counter++;

                            }

                            #endregion Material Process
                            #region Overhead  Input

                            bomOhs = new List<BOMOHVM>();
                            counter = 1;

                            foreach (DataRow row in OHOverheads)
                            {
                                BOMOHVM bomOh = new BOMOHVM();
                                decimal vRebateAmount = 0;
                                decimal vAdditionalCost = 0;
                                decimal vHeadAmount = 0;
                                decimal vAdditionalPercent = 0;

                                string overheadItemNo = "";

                                string overheadInputType = row["InputType"].ToString().Trim();
                                //string ohPacksize = row["Packsize"].ToString().Trim();
                                string overheadCode = row["Material_Code"].ToString().Trim();
                                string materialName = row["Material Name"].ToString().Trim();
                                //materialName = materialName.Replace(" ", "_");
                                Debug.WriteLine("Overhead Name '" + materialName + "' Finish Name '" + finishItemName + "' and code '" + pCode + "'");

                                if (string.IsNullOrEmpty(overheadInputType))
                                { throw new ArgumentNullException("BOMImport", "Cound not find input type for  ('" + materialName + "')  of Product ('" + finishItemName + "'('" + pCode + "'))"); }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(overheadCode))
                                    {
                                        overheadItemNo = productDal.GetProductNoByGroup_Code(overheadCode, "Overhead", connVM);
                                    }
                                    else
                                    {
                                        overheadItemNo = productDal.GetProductNoByGroup(materialName, "Overhead", connVM);
                                    }
                                }

                                var vRebate = row["Rebate%"].ToString().Trim();
                                if (string.IsNullOrEmpty(vRebate))
                                {
                                    vRebatePercent = 0;
                                }
                                else
                                {
                                    vRebatePercent = Convert.ToDouble(vRebate);
                                }

                                var vSTotal = row["SubTotal"].ToString().Trim().Trim();
                                if (string.IsNullOrEmpty(vSTotal))
                                {
                                    vSubTotal = 0;
                                }
                                else
                                {
                                    vSubTotal = Convert.ToDouble(vSTotal);
                                }

                                if (vRebatePercent == 0)
                                {
                                    vHeadAmount = Convert.ToDecimal(vSubTotal);
                                    vAdditionalCost = vHeadAmount;
                                    vAdditionalPercent = 100;

                                }
                                else
                                {
                                    vHeadAmount = Convert.ToDecimal(vSubTotal) * 100 / Convert.ToDecimal(vRebatePercent);
                                    vAdditionalCost = vHeadAmount - Convert.ToDecimal(vSubTotal);
                                    vAdditionalPercent = 100 - Convert.ToDecimal(vRebatePercent);

                                }


                                vRebateAmount = vHeadAmount - vAdditionalCost;
                                //vSubTotal,vRebatePercent,if (inputType == "Overhead")


                                bomOh.RebateAmount = vRebateAmount;
                                bomOh.AdditionalCost = vAdditionalCost;

                                bomOh.HeadName = materialName;
                                bomOh.HeadID = overheadItemNo;


                                bomOh.HeadAmount = vHeadAmount;
                                bomOh.CreatedBy = Program.CurrentUser;
                                bomOh.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                                bomOh.LastModifiedBy = Program.CurrentUser;
                                bomOh.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                                bomOh.FinishItemNo = finishItemNo;
                                bomOh.EffectDate = dateEffectDate.ToString("yyyy-MMM-dd");
                                bomOh.OHLineNo = "" + counter;
                                bomOh.RebatePercent = vAdditionalPercent;
                                bomOh.Post = "N";
                                vAdditionalTotal = vAdditionalTotal + vAdditionalCost;
                                bomOh.BranchId = Program.BranchId;
                                bomOhs.Add(bomOh);
                                counter++;

                            }
                            str1 = "";
                            #endregion Overhead  Input
                            #region Overhead Margin Input

                            BOMOHVM bomOh1 = new BOMOHVM();
                            bomOh1.HeadName = "Margin";
                            bomOh1.HeadID = "ovh0";

                            if (!string.IsNullOrEmpty(margin))
                                bomOh1.HeadAmount = Convert.ToDecimal(margin);
                            bomOh1.CreatedBy = Program.CurrentUser;
                            bomOh1.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                            bomOh1.LastModifiedBy = Program.CurrentUser;
                            bomOh1.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                            bomOh1.FinishItemNo = finishItemNo;
                            bomOh1.EffectDate = dateEffectDate.ToString("yyyy-MMM-dd");
                            bomOh1.OHLineNo = "100";
                            bomOh1.RebatePercent = 0;
                            bomOh1.Post = "N";
                            bomOh1.AdditionalCost = bomOh1.HeadAmount;
                            bomOh1.BranchId = Program.BranchId;
                            bomOhs.Add(bomOh1);
                            vAdditionalTotal = vAdditionalTotal + bomOh1.HeadAmount;

                            #endregion Overhead Margin Input
                            #region NBR

                            bomNbrs.RawTotal = vRawTotal;
                            bomNbrs.PackingTotal = vPackingTotal;
                            bomNbrs.RebateTotal = vRebateTotal;
                            bomNbrs.AdditionalTotal = vAdditionalTotal;
                            bomNbrs.RebateAdditionTotal = vAdditionalTotal + vRebateTotal;
                            bomNbrs.FirstSupplyDate = dtProduct.Rows[i]["FirstSupplyDate"].ToString();
                            var tt_TradeMarkup = vMarkupPercent;
                            var tt_SDRate = vSDRate;
                            var tt_VATRate = vVateRate;

                            string previousSDApplyedPrice = dtProduct.Rows[i]["PreviousSDApplyedPrice"].ToString().Trim();
                            if (!string.IsNullOrEmpty(previousSDApplyedPrice))
                                bomNbrs.LastNBRPrice = Convert.ToDecimal(previousSDApplyedPrice); //col 10

                            var tt_11P = vRawTotal + vPackingTotal + vRebateTotal + vAdditionalTotal;

                            var tt11 = tt_11P * tt_TradeMarkup / 100;
                            bomNbrs.PNBRPrice = tt11 + tt_11P; // col 11
                            var tt_12 = (tt11 + tt_11P) * tt_SDRate / 100;
                            bomNbrs.SDAmount = tt_12; // col 12

                            string prevNbrPrice = dtProduct.Rows[i]["PreviousVATABLE PRICE"].ToString().Trim();
                            if (!string.IsNullOrEmpty(prevNbrPrice))
                                bomNbrs.LastNBRWithSDAmount = Convert.ToDecimal(prevNbrPrice); //col 13



                            bomNbrs.NBRWithSDAmount = tt11 + tt_11P + tt_12; // col 14
                            var tt15 = (tt11 + tt_11P + tt_12) + ((tt11 + tt_11P + tt_12) * tt_VATRate / 100);
                            bomNbrs.WholeSalePrice = tt15;
                            if (!string.IsNullOrEmpty(packetPrice))
                                vPacketPrice = Convert.ToDecimal(packetPrice);
                            if (!string.IsNullOrEmpty(packetPrice))
                            {
                                bomNbrs.PPacketPrice = Convert.ToDecimal(packetPrice);
                            }

                            // col 16

                            bomNbrs.IsImported = "Y";
                            if (Vat_Name == "VAT 4.3 (Toll Issue)")
                            {
                                bomNbrs.RawOHCost = vRebateTotal;
                            }
                            else
                            {
                                bomNbrs.RawOHCost = vRawTotal + vPackingTotal + vRebateTotal;

                            }
                            bomNbrs.CreatedBy = Program.CurrentUser;
                            bomNbrs.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                            bomNbrs.LastModifiedBy = Program.CurrentUser;
                            bomNbrs.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                            bomNbrs.ActiveStatus = "Y";
                            bomNbrs.Post = "N";
                            bomNbrs.CustomerID = "0";
                            bomNbrs.BranchId = Program.BranchId;
                            #endregion NBR


                            //  var ohs = bomOhs.Where(x => x.HeadName.ToLower() == "gas").ToList();
                            // var ohs = bomOhs.Where(x => x.HeadName.ToLower() == "gas").ToList();



                            sqlResults = bom1.BOMInsert(bomItems, bomOhs, bomNbrs);
                            if (sqlResults[0] == "Success")
                            {
                                MessageBox.Show("Peice decleration of Product ('" + finishItemName + "'('" + pCode + "') and VAT name ('" + bomNbrs.VATName + "') successsfully complete");
                            }
                            else
                            {
                                MessageBox.Show(sqlResults[1]);
                            }
                        }
                        #endregion Process model
                    }
                }

            }
            #endregion
            catch (Exception ex)
            {
                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
                FileLogger.Log("FormMasterImport", "bgwReceiveSave", ex.Message);

            }
        }

        private void bgwBOMSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ShowingMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private DataTable GetTableFromText(string TableName)
        {
            #region Variables
            DataTable dt = new DataTable();
            string files = Program.ImportFileName;
            #endregion
            if (string.IsNullOrEmpty(files))
            {
                return dt;
            }
            string[] fileNames = files.Split(";".ToCharArray());
            StreamReader sr = new StreamReader(fileNames[0]);
            #region Puchase
            if (TableName == CONST_PURCHASETYPE)
            {
                DataTable dtPurchaseM = new DataTable();
                DataTable dtPurchaseD = new DataTable();
                #region Master table
                dtPurchaseM.Columns.Add("Identifier");
                dtPurchaseM.Columns.Add("ID");
                dtPurchaseM.Columns.Add("Vendor_Code");
                dtPurchaseM.Columns.Add("Vendor_Name");
                dtPurchaseM.Columns.Add("Referance_No");
                dtPurchaseM.Columns.Add("LC_No");
                dtPurchaseM.Columns.Add("BE_Number");
                dtPurchaseM.Columns.Add("Invoice_Date");
                dtPurchaseM.Columns.Add("Receive_Date");
                dtPurchaseM.Columns.Add("Post");
                dtPurchaseM.Columns.Add("With_VDS");
                dtPurchaseM.Columns.Add("Previous_Purchase_No");
                dtPurchaseM.Columns.Add("Comments");
                dtPurchaseM.Columns.Add("Custom_House");
                dtPurchaseM.Columns.Add("Transection_Type").DefaultValue = transactionType;
                dtPurchaseM.Columns.Add("LCDate");
                dtPurchaseM.Columns.Add("LandedCost");
                dtPurchaseM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
                dtPurchaseM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;

                #endregion Master table
                #region Details table
                dtPurchaseD.Columns.Add("Identifier");
                dtPurchaseD.Columns.Add("ID");
                dtPurchaseD.Columns.Add("Item_Code");
                dtPurchaseD.Columns.Add("Item_Name");
                dtPurchaseD.Columns.Add("Quantity");
                dtPurchaseD.Columns.Add("Total_Price");
                dtPurchaseD.Columns.Add("UOM");
                dtPurchaseD.Columns.Add("Type");
                dtPurchaseD.Columns.Add("Rebate_Rate");
                dtPurchaseD.Columns.Add("SD_Amount");
                dtPurchaseD.Columns.Add("VAT_Amount");
                #endregion Details table
                foreach (var fileName in fileNames)
                {
                    sr = new StreamReader(fileName);
                    string masterData = sr.ReadToEnd();
                    string[] masterRows = masterData.Split("\r".ToCharArray());
                    string delimeter = "|";
                    string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper();
                    if (recordType == "M")
                    {
                        foreach (string mRow in masterRows)
                        {
                            string[] mItems = mRow.Split(delimeter.ToCharArray());
                            if (mItems[0].Replace("\n", "").ToUpper() == "M")
                            {
                                dtPurchaseM.Rows.Add(mItems);
                            }
                        }
                    }
                    else if (recordType == "D")
                    {
                        foreach (string dRow in masterRows)
                        {
                            string[] dItems = dRow.Split(delimeter.ToCharArray());
                            if (dItems[0].Replace("\n", "").ToUpper() == "D")
                            {
                                dtPurchaseD.Rows.Add(dItems);
                            }
                        }
                    }
                }
                if (sr != null)
                {
                    sr.Close();
                }

                ds = new DataSet();
                ds.Tables.Add(dtPurchaseM);
                ds.Tables.Add(dtPurchaseD);
                dt = dtPurchaseM;
            }
            #endregion

            #region Sale
            else if (TableName == CONST_SALETYPE)
            {
                DataTable dtSaleM = new DataTable();
                DataTable dtSaleD = new DataTable();
                DataTable dtSaleE = new DataTable();

                #region Master table
                dtSaleM.Columns.Add("Identifier");
                dtSaleM.Columns.Add("ID");
                dtSaleM.Columns.Add("Customer_Code");
                dtSaleM.Columns.Add("Customer_Name");
                dtSaleM.Columns.Add("Delivery_Address");
                dtSaleM.Columns.Add("Vehicle_No");
                dtSaleM.Columns.Add("Invoice_Date_Time");
                dtSaleM.Columns.Add("Delivery_Date_Time");
                dtSaleM.Columns.Add("Reference_No");
                dtSaleM.Columns.Add("Sale_Type");
                dtSaleM.Columns.Add("Previous_Invoice_No");
                dtSaleM.Columns.Add("Is_Print");
                dtSaleM.Columns.Add("Tender_Id");
                dtSaleM.Columns.Add("Post");
                dtSaleM.Columns.Add("LC_Number");
                dtSaleM.Columns.Add("Currency_Code");
                dtSaleM.Columns.Add("Transection_Type").DefaultValue = transactionType;
                dtSaleM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
                dtSaleM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;
                dtSaleM.Columns.Add("Comments");



                #endregion Master table

                #region Details table
                dtSaleD.Columns.Add("Identifier");
                dtSaleD.Columns.Add("ID");
                dtSaleD.Columns.Add("Item_Code");
                dtSaleD.Columns.Add("Item_Name");
                dtSaleD.Columns.Add("Quantity");
                dtSaleD.Columns.Add("NBR_Price");
                dtSaleD.Columns.Add("UOM");
                dtSaleD.Columns.Add("VAT_Rate");
                dtSaleD.Columns.Add("SD_Rate");
                dtSaleD.Columns.Add("Non_Stock");
                dtSaleD.Columns.Add("Trading_MarkUp");
                dtSaleD.Columns.Add("Type");
                dtSaleD.Columns.Add("Discount_Amount");
                dtSaleD.Columns.Add("Promotional_Quantity");
                dtSaleD.Columns.Add("VAT_Name");
                dtSaleD.Columns.Add("Weight");
                dtSaleD.Columns.Add("Total_Price");
                dtSaleD.Columns.Add("TotalValue");

                dtSaleD.Columns.Add("WareHouseRent");
                dtSaleD.Columns.Add("WareHouseVAT");
                dtSaleD.Columns.Add("ATVRate");
                dtSaleD.Columns.Add("ATVablePrice");
                dtSaleD.Columns.Add("ATVAmount");
                dtSaleD.Columns.Add("IsCommercialImporter");
                dtSaleD.Columns.Add("ValueOnly");

                #endregion Details table

                foreach (var fileName in fileNames)
                {
                    sr = new StreamReader(fileName);
                    string masterData = sr.ReadToEnd();
                    string[] masterRows = masterData.Split("\r".ToCharArray());
                    string delimeter = "|";
                    string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper();
                    if (recordType == "M")
                    {
                        foreach (string mRow in masterRows)
                        {
                            string[] mItems = mRow.Split(delimeter.ToCharArray());
                            if (mItems[0].Replace("\n", "").ToUpper() == "M")
                            {
                                dtSaleM.Rows.Add(mItems);
                            }
                        }
                    }
                    else if (recordType == "D")
                    {
                        foreach (string dRow in masterRows)
                        {
                            string[] dItems = dRow.Split(delimeter.ToCharArray());
                            if (dItems[0].Replace("\n", "").ToUpper() == "D")
                            {
                                dtSaleD.Rows.Add(dItems);
                            }
                        }
                    }
                }

                if (sr != null)
                {
                    sr.Close();
                }
                foreach (DataRow row in dtSaleM.Rows)
                {
                    row["Transection_Type"] = transactionType;
                }
                ds = new DataSet();
                ds.Tables.Add(dtSaleM);
                ds.Tables.Add(dtSaleD);
                ds.Tables.Add(dtSaleE);
                dt = dtSaleM;
            }
            #endregion

            #region Issue
            else if (TableName == CONST_ISSUETYPE)
            {
                DataTable dtIssueM = new DataTable();
                DataTable dtIssueD = new DataTable();

                #region Master table
                dtIssueM.Columns.Add("Identifier");
                dtIssueM.Columns.Add("ID");
                dtIssueM.Columns.Add("Issue_DateTime");
                dtIssueM.Columns.Add("Reference_No");
                dtIssueM.Columns.Add("Comments");
                dtIssueM.Columns.Add("Return_Id");
                dtIssueM.Columns.Add("Post");
                dtIssueM.Columns.Add("Transection_Type").DefaultValue = transactionType;
                dtIssueM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
                dtIssueM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;

                #endregion Master table

                #region Details table
                dtIssueD.Columns.Add("Identifier");
                dtIssueD.Columns.Add("ID");
                dtIssueD.Columns.Add("Item_Code");
                dtIssueD.Columns.Add("Item_Name");
                dtIssueD.Columns.Add("Quantity");
                dtIssueD.Columns.Add("UOM");

                #endregion Details table

                foreach (var fileName in fileNames)
                {
                    sr = new StreamReader(fileName);
                    string masterData = sr.ReadToEnd();
                    string[] masterRows = masterData.Split("\r".ToCharArray());
                    string delimeter = "|";
                    string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper();
                    if (recordType == "M")
                    {
                        foreach (string mRow in masterRows)
                        {
                            string[] mItems = mRow.Split(delimeter.ToCharArray());
                            if (mItems[0].Replace("\n", "").ToUpper() == "M")
                            {
                                dtIssueM.Rows.Add(mItems);
                            }
                        }
                    }
                    else if (recordType == "D")
                    {
                        foreach (string dRow in masterRows)
                        {
                            string[] dItems = dRow.Split(delimeter.ToCharArray());
                            if (dItems[0].Replace("\n", "").ToUpper() == "D")
                            {
                                dtIssueD.Rows.Add(dItems);
                            }
                        }
                    }
                }

                if (sr != null)
                {
                    sr.Close();
                }
                ds = new DataSet();
                ds.Tables.Add(dtIssueM);
                ds.Tables.Add(dtIssueD);
                dt = dtIssueM;
            }
            #endregion

            #region Receive
            else if (TableName == CONST_RECEIVETYPE)
            {
                DataTable dtReceiveM = new DataTable();
                DataTable dtReceiveD = new DataTable();
                string IssueFromBOM = new CommonDAL().settingsDesktop("IssueFromBOM", "IssueFromBOM");
                #region Master table
                dtReceiveM.Columns.Add("Identifier");
                dtReceiveM.Columns.Add("ID");
                dtReceiveM.Columns.Add("Receive_DateTime");
                dtReceiveM.Columns.Add("Reference_No");
                dtReceiveM.Columns.Add("Comments");
                dtReceiveM.Columns.Add("Post");
                dtReceiveM.Columns.Add("Return_Id");
                dtReceiveM.Columns.Add("CustomerID");
                dtReceiveM.Columns.Add("Transection_Type").DefaultValue = transactionType;
                dtReceiveM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
                dtReceiveM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;
                dtReceiveM.Columns.Add("From_BOM").DefaultValue = IssueFromBOM;
                dtReceiveM.Columns.Add("Total_VAT_Amount").DefaultValue = "0";
                dtReceiveM.Columns.Add("Total_Amount").DefaultValue = "0";
                #endregion Master table

                #region Details table
                dtReceiveD.Columns.Add("Identifier");
                dtReceiveD.Columns.Add("ID");
                dtReceiveD.Columns.Add("Item_Code");
                dtReceiveD.Columns.Add("Item_Name");
                dtReceiveD.Columns.Add("Quantity");
                dtReceiveD.Columns.Add("NBR_Price");
                dtReceiveD.Columns.Add("UOM");
                dtReceiveD.Columns.Add("VAT_Name");
                #endregion Details table

                foreach (var fileName in fileNames)
                {
                    sr = new StreamReader(fileName);
                    string masterData = sr.ReadToEnd();
                    string[] masterRows = masterData.Split("\r".ToCharArray());
                    string delimeter = "|";
                    string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper().Trim();
                    if (recordType == "M")
                    {
                        foreach (string mRow in masterRows)
                        {
                            string[] mItems = mRow.Split(delimeter.ToCharArray());
                            if (mItems[0].Replace("\n", "").ToUpper().Trim() == "M")
                            {
                                dtReceiveM.Rows.Add(mItems);
                            }
                        }
                    }
                    else
                    {
                        foreach (string dRow in masterRows)
                        {
                            string[] dItems = dRow.Split(delimeter.ToCharArray());
                            if (dItems[0].Replace("\n", "").ToUpper().Trim() == "D")
                            {
                                dtReceiveD.Rows.Add(dItems);
                            }
                        }
                    }
                }

                if (sr != null)
                {
                    sr.Close();
                }
                ds = new DataSet();
                ds.Tables.Add(dtReceiveM);
                ds.Tables.Add(dtReceiveD);
                dt = dtReceiveM;

            }
            #endregion

            #region VDS
            else if (TableName == CONST_VDSTYPE)
            {
                MessageBox.Show(this, "No text implementation for VDS done yet!");
                return dt;
            }
            #endregion

            #region BOM
            else if (TableName == CONST_BOMTYPE)
            {
                MessageBox.Show(this, "No text implementation for VDS done yet!");
                return dt;
            }
            #endregion

            return dt;
        }

        private DataTable GetTableFromTextDouble(string TableName)
        {
            #region Variables
            DataTable dt = new DataTable();
            string files = Program.ImportFileName;
            #endregion
            if (string.IsNullOrEmpty(files))
            {
                return dt;
            }
            string[] fileNames = files.Split(";".ToCharArray());
            StreamReader sr = new StreamReader(fileNames[0]);
            try
            {
                #region Puchase
                if (TableName == CONST_PURCHASETYPE)
                {
                    DataTable dtPurchaseM = new DataTable();
                    DataTable dtPurchaseD = new DataTable();
                    #region Master table
                    dtPurchaseM.Columns.Add("Identifier");
                    dtPurchaseM.Columns.Add("ID");
                    dtPurchaseM.Columns.Add("Vendor_Code");
                    dtPurchaseM.Columns.Add("Vendor_Name");
                    dtPurchaseM.Columns.Add("Referance_No");
                    dtPurchaseM.Columns.Add("LC_No");
                    dtPurchaseM.Columns.Add("BE_Number");
                    dtPurchaseM.Columns.Add("Invoice_Date");
                    dtPurchaseM.Columns.Add("Receive_Date");
                    dtPurchaseM.Columns.Add("Post");
                    dtPurchaseM.Columns.Add("With_VDS");
                    dtPurchaseM.Columns.Add("Previous_Purchase_No");
                    dtPurchaseM.Columns.Add("Comments");
                    dtPurchaseM.Columns.Add("Custom_House");
                    dtPurchaseM.Columns.Add("Transection_Type").DefaultValue = transactionType;
                    dtPurchaseM.Columns.Add("LCDate");
                    dtPurchaseM.Columns.Add("LandedCost");
                    dtPurchaseM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
                    dtPurchaseM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;

                    #endregion Master table
                    #region Details table
                    dtPurchaseD.Columns.Add("Identifier");
                    dtPurchaseD.Columns.Add("ID");
                    dtPurchaseD.Columns.Add("Item_Code");
                    dtPurchaseD.Columns.Add("Item_Name");
                    dtPurchaseD.Columns.Add("Quantity");
                    dtPurchaseD.Columns.Add("Total_Price");
                    dtPurchaseD.Columns.Add("UOM");
                    dtPurchaseD.Columns.Add("Type");
                    dtPurchaseD.Columns.Add("Rebate_Rate");
                    dtPurchaseD.Columns.Add("SD_Amount");
                    dtPurchaseD.Columns.Add("VAT_Amount");
                    #endregion Details table
                    foreach (var fileName in fileNames)
                    {
                        sr = new StreamReader(fileName);
                        string masterData = sr.ReadToEnd();
                        string[] masterRows = masterData.Split("\r".ToCharArray());
                        string delimeter = "|";
                        string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper();
                        if (recordType == "M")
                        {
                            foreach (string mRow in masterRows)
                            {
                                string[] mItems = mRow.Split(delimeter.ToCharArray());
                                if (mItems[0].Replace("\n", "").ToUpper() == "M")
                                {
                                    dtPurchaseM.Rows.Add(mItems);
                                }
                            }
                        }
                        else if (recordType == "D")
                        {
                            foreach (string dRow in masterRows)
                            {
                                string[] dItems = dRow.Split(delimeter.ToCharArray());
                                if (dItems[0].Replace("\n", "").ToUpper() == "D")
                                {
                                    dtPurchaseD.Rows.Add(dItems);
                                }
                            }
                        }
                    }
                    if (sr != null)
                    {
                        sr.Close();
                    }

                    ds = new DataSet();
                    ds.Tables.Add(dtPurchaseM);
                    ds.Tables.Add(dtPurchaseD);

                    PopulatePurchase(dt);
                    MergeToSingle(dtPurchaseM, dtPurchaseD, dt);
                }
                #endregion

                #region Sale
                else if (TableName == CONST_SALETYPE)
                {
                    DataTable dtSaleM = new DataTable();
                    DataTable dtSaleD = new DataTable();
                    DataTable dtSaleE = new DataTable();

                    #region Master table
                    dtSaleM.Columns.Add("Identifier");
                    dtSaleM.Columns.Add("ID");
                    dtSaleM.Columns.Add("Customer_Code");
                    dtSaleM.Columns.Add("Customer_Name");
                    dtSaleM.Columns.Add("Delivery_Address");
                    dtSaleM.Columns.Add("Vehicle_No");
                    dtSaleM.Columns.Add("Invoice_Date_Time");
                    dtSaleM.Columns.Add("Delivery_Date_Time");
                    dtSaleM.Columns.Add("Reference_No");
                    dtSaleM.Columns.Add("Sale_Type");
                    dtSaleM.Columns.Add("Previous_Invoice_No");
                    dtSaleM.Columns.Add("Is_Print");
                    dtSaleM.Columns.Add("Tender_Id");
                    dtSaleM.Columns.Add("Post");
                    dtSaleM.Columns.Add("LC_Number");
                    dtSaleM.Columns.Add("Currency_Code");
                    dtSaleM.Columns.Add("Transection_Type").DefaultValue = transactionType;
                    dtSaleM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
                    dtSaleM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;
                    dtSaleM.Columns.Add("Comments");



                    #endregion Master table

                    #region Details table
                    dtSaleD.Columns.Add("Identifier");
                    dtSaleD.Columns.Add("ID");
                    dtSaleD.Columns.Add("Item_Code");
                    dtSaleD.Columns.Add("Item_Name");
                    dtSaleD.Columns.Add("Quantity");
                    dtSaleD.Columns.Add("NBR_Price");
                    dtSaleD.Columns.Add("UOM");
                    dtSaleD.Columns.Add("VAT_Rate");
                    dtSaleD.Columns.Add("SD_Rate");
                    dtSaleD.Columns.Add("Non_Stock");
                    dtSaleD.Columns.Add("Trading_MarkUp");
                    dtSaleD.Columns.Add("Type");
                    dtSaleD.Columns.Add("Discount_Amount");
                    dtSaleD.Columns.Add("Promotional_Quantity");
                    dtSaleD.Columns.Add("VAT_Name");
                    dtSaleD.Columns.Add("Weight");
                    dtSaleD.Columns.Add("Total_Price");
                    dtSaleD.Columns.Add("TotalValue");

                    dtSaleD.Columns.Add("WareHouseRent");
                    dtSaleD.Columns.Add("WareHouseVAT");
                    dtSaleD.Columns.Add("ATVRate");
                    dtSaleD.Columns.Add("ATVablePrice");
                    dtSaleD.Columns.Add("ATVAmount");
                    dtSaleD.Columns.Add("IsCommercialImporter");
                    dtSaleD.Columns.Add("ValueOnly");

                    #endregion Details table

                    foreach (var fileName in fileNames)
                    {
                        sr = new StreamReader(fileName);
                        string masterData = sr.ReadToEnd();
                        string[] masterRows = masterData.Split("\r".ToCharArray());
                        string delimeter = "|";
                        string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper();
                        if (recordType == "M")
                        {
                            foreach (string mRow in masterRows)
                            {
                                string[] mItems = mRow.Split(delimeter.ToCharArray());
                                if (mItems[0].Replace("\n", "").ToUpper() == "M")
                                {
                                    dtSaleM.Rows.Add(mItems);
                                }
                            }
                        }
                        else if (recordType == "D")
                        {
                            foreach (string dRow in masterRows)
                            {
                                string[] dItems = dRow.Split(delimeter.ToCharArray());
                                if (dItems[0].Replace("\n", "").ToUpper() == "D")
                                {
                                    dtSaleD.Rows.Add(dItems);
                                }
                            }
                        }
                    }

                    if (sr != null)
                    {
                        sr.Close();
                    }
                    foreach (DataRow row in dtSaleM.Rows)
                    {
                        row["Transection_Type"] = transactionType;
                    }
                    ds = new DataSet();
                    ds.Tables.Add(dtSaleM);
                    ds.Tables.Add(dtSaleD);
                    ds.Tables.Add(dtSaleE);
                    dt = dtSaleM;
                }
                #endregion

                #region Issue
                else if (TableName == CONST_ISSUETYPE)
                {
                    DataTable dtIssueM = new DataTable();
                    DataTable dtIssueD = new DataTable();

                    #region Master table
                    dtIssueM.Columns.Add("Identifier");
                    dtIssueM.Columns.Add("ID");
                    dtIssueM.Columns.Add("Issue_DateTime");
                    dtIssueM.Columns.Add("Reference_No");
                    dtIssueM.Columns.Add("Comments");
                    dtIssueM.Columns.Add("Return_Id");
                    dtIssueM.Columns.Add("Post");
                    dtIssueM.Columns.Add("Transection_Type").DefaultValue = transactionType;
                    dtIssueM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
                    dtIssueM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;

                    #endregion Master table

                    #region Details table
                    dtIssueD.Columns.Add("Identifier");
                    dtIssueD.Columns.Add("ID");
                    dtIssueD.Columns.Add("Item_Code");
                    dtIssueD.Columns.Add("Item_Name");
                    dtIssueD.Columns.Add("Quantity");
                    dtIssueD.Columns.Add("UOM");

                    #endregion Details table

                    foreach (var fileName in fileNames)
                    {
                        sr = new StreamReader(fileName);
                        string masterData = sr.ReadToEnd();
                        string[] masterRows = masterData.Split("\r".ToCharArray());
                        string delimeter = "|";
                        string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper();
                        if (recordType == "M")
                        {
                            foreach (string mRow in masterRows)
                            {
                                string[] mItems = mRow.Split(delimeter.ToCharArray());
                                if (mItems[0].Replace("\n", "").ToUpper() == "M")
                                {
                                    dtIssueM.Rows.Add(mItems);
                                }
                            }
                        }
                        else if (recordType == "D")
                        {
                            foreach (string dRow in masterRows)
                            {
                                string[] dItems = dRow.Split(delimeter.ToCharArray());
                                if (dItems[0].Replace("\n", "").ToUpper() == "D")
                                {
                                    dtIssueD.Rows.Add(dItems);
                                }
                            }
                        }
                    }

                    if (sr != null)
                    {
                        sr.Close();
                    }
                    ds = new DataSet();
                    ds.Tables.Add(dtIssueM);
                    ds.Tables.Add(dtIssueD);
                    dt = dtIssueM;
                }
                #endregion

                #region Receive
                else if (TableName == CONST_RECEIVETYPE)
                {
                    DataTable dtReceiveM = new DataTable();
                    DataTable dtReceiveD = new DataTable();
                    string IssueFromBOM = new CommonDAL().settingsDesktop("IssueFromBOM", "IssueFromBOM", null, connVM);
                    #region Master table
                    dtReceiveM.Columns.Add("Identifier");
                    dtReceiveM.Columns.Add("ID");
                    dtReceiveM.Columns.Add("Receive_DateTime");
                    dtReceiveM.Columns.Add("Reference_No");
                    dtReceiveM.Columns.Add("Comments");
                    dtReceiveM.Columns.Add("Post");
                    dtReceiveM.Columns.Add("Return_Id");
                    dtReceiveM.Columns.Add("CustomerCode");
                    dtReceiveM.Columns.Add("Transection_Type").DefaultValue = transactionType;
                    dtReceiveM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
                    dtReceiveM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;
                    dtReceiveM.Columns.Add("From_BOM").DefaultValue = IssueFromBOM;
                    dtReceiveM.Columns.Add("Total_VAT_Amount").DefaultValue = "0";
                    dtReceiveM.Columns.Add("Total_Amount").DefaultValue = "0";
                    #endregion Master table

                    #region Details table
                    dtReceiveD.Columns.Add("Identifier");
                    dtReceiveD.Columns.Add("ID");
                    dtReceiveD.Columns.Add("Item_Code");
                    dtReceiveD.Columns.Add("Item_Name");
                    dtReceiveD.Columns.Add("Quantity");
                    dtReceiveD.Columns.Add("NBR_Price");
                    dtReceiveD.Columns.Add("UOM");
                    dtReceiveD.Columns.Add("VAT_Name");
                    #endregion Details table

                    foreach (var fileName in fileNames)
                    {
                        sr = new StreamReader(fileName);
                        string masterData = sr.ReadToEnd();
                        string[] masterRows = masterData.Split("\r".ToCharArray());
                        string delimeter = "|";
                        string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper().Trim();
                        if (recordType == "M")
                        {
                            foreach (string mRow in masterRows)
                            {
                                string[] mItems = mRow.Split(delimeter.ToCharArray());
                                if (mItems[0].Replace("\n", "").ToUpper().Trim() == "M")
                                {
                                    dtReceiveM.Rows.Add(mItems);
                                }
                            }
                        }
                        else
                        {
                            foreach (string dRow in masterRows)
                            {
                                string[] dItems = dRow.Split(delimeter.ToCharArray());
                                if (dItems[0].Replace("\n", "").ToUpper().Trim() == "D")
                                {
                                    dtReceiveD.Rows.Add(dItems);
                                }
                            }
                        }
                    }

                    if (sr != null)
                    {
                        sr.Close();
                    }
                    ds = new DataSet();
                    //ds.Tables.Add(dtReceiveM);
                    //ds.Tables.Add(dtReceiveD);
                    PopulateReceive(dt);

                    MergeToSingle(dtReceiveM, dtReceiveD, dt);

                }
                #endregion

                #region VDS
                else if (TableName == CONST_VDSTYPE)
                {
                    MessageBox.Show(this, "No text implementation for VDS done yet!");
                    return dt;
                }
                #endregion

                #region BOM
                else if (TableName == CONST_BOMTYPE)
                {
                    MessageBox.Show(this, "No text implementation for VDS done yet!");
                    return dt;
                }
                #endregion
            }
            catch (Exception e)
            {

            }

            return dt;
        }

        private void PopulatePurchase(DataTable table)
        {
            table.Rows.Clear();
            table.Columns.Clear();

            table.Columns.Add("ID");
            table.Columns.Add("BranchCode");
            table.Columns.Add("Vendor_Code");
            table.Columns.Add("Vendor_Name");
            table.Columns.Add("Referance_No");
            table.Columns.Add("LC_No");
            table.Columns.Add("BE_Number");
            table.Columns.Add("Invoice_Date");
            table.Columns.Add("Receive_Date");
            table.Columns.Add("Post");
            table.Columns.Add("With_VDS");
            table.Columns.Add("Previous_Purchase_No");
            table.Columns.Add("Comments");
            table.Columns.Add("Custom_House");

            table.Columns.Add("Item_Name");
            table.Columns.Add("Item_Code");

            table.Columns.Add("Quantity");
            table.Columns.Add("Total_Price");
            table.Columns.Add("UOM");
            table.Columns.Add("Type");
            table.Columns.Add("Rebate_Rate");
            table.Columns.Add("SD_Amount");
            table.Columns.Add("VAT_Amount");
            table.Columns.Add("CnF_Amount");

            table.Columns.Add("Insurance_Amount");
            table.Columns.Add("Assessable_Value");
            table.Columns.Add("CD_Amount");
            table.Columns.Add("RD_Amount");
            table.Columns.Add("TVB_Amount");
            table.Columns.Add("TVA_Amount");
            table.Columns.Add("ATV_Amount");
            table.Columns.Add("Others_Amount");
            table.Columns.Add("Remarks");
        }

        private void PopulateReceive(DataTable table)
        {
            table.Rows.Clear();
            table.Columns.Clear();

            table.Columns.Add("ID");
            table.Columns.Add("BranchCode");
            table.Columns.Add("Receive_DateTime");
            table.Columns.Add("Reference_No");
            table.Columns.Add("Comments");
            table.Columns.Add("Post");
            table.Columns.Add("Return_Id");

            table.Columns.Add("Item_Code");
            table.Columns.Add("Item_Name");
            table.Columns.Add("Quantity");
            table.Columns.Add("NBR_Price");
            table.Columns.Add("UOM");
            table.Columns.Add("VAT_Name");
            table.Columns.Add("CustomerCode");
        }

        private void MergeToSingle(DataTable dtM, DataTable dtD, DataTable dt)
        {
            try
            {
                var j = 0;
                foreach (DataRow row in dtM.Rows)
                {
                    var dataRows = dtD.Select("ID=" + "'" + row["ID"] + "'");

                    var details = dataRows.CopyToDataTable();

                    foreach (DataRow detail in details.Rows)
                    {
                        dt.Rows.Add(dt.NewRow());

                        var columnsCount = dt.Columns.Count;

                        for (var i = 0; i < columnsCount; i++)
                        {
                            var columnName = dt.Columns[i].ColumnName;

                            if (dtM.Columns.Contains(columnName))
                            {
                                dt.Rows[j][columnName] = row[columnName];
                            }
                            else if (dtD.Columns.Contains(columnName))
                            {
                                dt.Rows[j][columnName] = detail[columnName];
                            }
                        }

                        j++;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private DataTable GetTableFromSingleText(string TableName)
        {
            #region Variables
            DataTable dt = new DataTable();
            string files = Program.ImportFileName;
            #endregion
            if (string.IsNullOrEmpty(files))
            {
                return dt;
            }
            string[] fileNames = files.Split(";".ToCharArray());
            StreamReader sr = null;
            #region Puchase
            if (TableName == CONST_PURCHASETYPE)
            {
                dt = GetPurchase(fileNames);
            }
            #endregion

            #region Sale
            else if (TableName == CONST_SALETYPE)
            {
                DataTable dtSaleM = new DataTable();
                DataTable dtSaleD = new DataTable();
                DataTable dtSaleE = new DataTable();

                #region Master table
                dtSaleM.Columns.Add("ID");
                dtSaleM.Columns.Add("Branch_Code");
                dtSaleM.Columns.Add("CustomerGroup");
                dtSaleM.Columns.Add("Customer_Name");

                dtSaleM.Columns.Add("Customer_Code");
                dtSaleM.Columns.Add("Delivery_Address");
                dtSaleM.Columns.Add("Invoice_Date_Time");
                dtSaleM.Columns.Add("Delivery_Date_Time");
                dtSaleM.Columns.Add("Reference_No");
                dtSaleM.Columns.Add("Comments");
                dtSaleM.Columns.Add("Sale_Type");
                dtSaleM.Columns.Add("Previous_Invoice_No");
                dtSaleM.Columns.Add("Is_Print");
                dtSaleM.Columns.Add("Tender_Id");
                dtSaleM.Columns.Add("Post");
                dtSaleM.Columns.Add("LC_Number");
                dtSaleM.Columns.Add("Currency_Code");
                dtSaleM.Columns.Add("Item_Code");
                dtSaleM.Columns.Add("Item_Name");

                dtSaleM.Columns.Add("Quantity");
                dtSaleM.Columns.Add("NBR_Price");
                dtSaleM.Columns.Add("UOM");
                dtSaleM.Columns.Add("VAT_Rate");
                dtSaleM.Columns.Add("SD_Rate");
                dtSaleM.Columns.Add("Non_Stock");
                dtSaleM.Columns.Add("Trading_MarkUp");
                dtSaleM.Columns.Add("Type");
                dtSaleM.Columns.Add("Discount_Amount");
                dtSaleM.Columns.Add("Promotional_Quantity");
                dtSaleM.Columns.Add("VAT_Name");
                dtSaleM.Columns.Add("SubTotal");

                dtSaleM.Columns.Add("Vehicle_No");



                dtSaleM.Columns.Add("ExpDescription");

                dtSaleM.Columns.Add("ExpQuantity");

                dtSaleM.Columns.Add("ExpGrossWeight");
                dtSaleM.Columns.Add("ExpNetWeight");
                dtSaleM.Columns.Add("ExpNumberFrom");
                dtSaleM.Columns.Add("ExpNumberTo");


                #endregion Master table



                var fileName = fileNames[0];

                sr = new StreamReader(fileName);
                string masterData = sr.ReadToEnd();
                string[] masterRows = masterData.Split("\r".ToCharArray());
                string delimeter = "|";

                foreach (string mRow in masterRows)
                {
                    string[] mItems = mRow.Replace("\n", "").Split(delimeter.ToCharArray());
                    dtSaleM.Rows.Add(mItems);
                }

                if (sr != null)
                {
                    sr.Close();
                }

                dt = dtSaleM;
            }
            #endregion

            #region Issue
            else if (TableName == CONST_ISSUETYPE)
            {
                dt = GetIssue(fileNames);
            }
            #endregion

            #region Receive
            else if (TableName == CONST_RECEIVETYPE)
            {
                var dtReceiveM = GetReceiveData(fileNames);

                dt = dtReceiveM;
            }
            #endregion

            #region VDS
            else if (TableName == CONST_VDSTYPE)
            {
                MessageBox.Show(this, "No text implementation for VDS done yet!");
                return dt;
            }
            #endregion

            #region BOM
            else if (TableName == CONST_BOMTYPE)
            {
                MessageBox.Show(this, "No text implementation for VDS done yet!");
                return dt;
            }
            #endregion

            #region TransferIssue
            else if (TableName == CONST_TRANSFERTYPE)
            {
                DataTable dtTransferIssue = new DataTable();


                dtTransferIssue.Columns.Add("ID");
                dtTransferIssue.Columns.Add("BranchCode");
                dtTransferIssue.Columns.Add("TransactionDateTime");
                dtTransferIssue.Columns.Add("TransactionType");
                dtTransferIssue.Columns.Add("ProductCode");
                dtTransferIssue.Columns.Add("ProductName");
                dtTransferIssue.Columns.Add("UOM");
                dtTransferIssue.Columns.Add("Quantity");
                dtTransferIssue.Columns.Add("TransferToBranchCode");
                dtTransferIssue.Columns.Add("CostPrice");
                dtTransferIssue.Columns.Add("Post");
                dtTransferIssue.Columns.Add("VAT_Rate");
                dtTransferIssue.Columns.Add("ReferenceNo");
                dtTransferIssue.Columns.Add("Comments");
                dtTransferIssue.Columns.Add("CommentsD");


                var fileName = fileNames[0];

                sr = new StreamReader(fileName);
                string masterData = sr.ReadToEnd();
                string[] masterRows = masterData.Split("\r".ToCharArray());
                string delimeter = "|";

                foreach (string mRow in masterRows)
                {
                    string[] mItems = mRow.Replace("\n", "").Split(delimeter.ToCharArray());
                    dtTransferIssue.Rows.Add(mItems);
                }

                if (sr != null)
                {
                    sr.Close();
                }

                dt = dtTransferIssue;
            }
            #endregion

            return dt;
        }

        private DataTable GetIssue(string[] fileNames)
        {
            StreamReader sr;
            DataTable dt;
            DataTable dtIssueM = new DataTable();

            dtIssueM.Columns.Add("ID");
            dtIssueM.Columns.Add("BranchCode");
            dtIssueM.Columns.Add("Issue_DateTime");
            dtIssueM.Columns.Add("Reference_No");
            dtIssueM.Columns.Add("Comments");
            dtIssueM.Columns.Add("Return_Id");
            dtIssueM.Columns.Add("Post");

            dtIssueM.Columns.Add("Item_Code");
            dtIssueM.Columns.Add("Item_Name");
            dtIssueM.Columns.Add("Quantity");
            dtIssueM.Columns.Add("UOM");


            var fileName = fileNames[0];

            sr = new StreamReader(fileName);
            string masterData = sr.ReadToEnd();
            string[] masterRows = masterData.Split("\r".ToCharArray());
            string delimeter = "|";

            foreach (string mRow in masterRows)
            {
                string[] mItems = mRow.Replace("\n", "").Split(delimeter.ToCharArray());
                dtIssueM.Rows.Add(mItems);
            }

            if (sr != null)
            {
                sr.Close();
            }

            dt = dtIssueM;
            return dt;
        }

        private DataTable GetReceiveData(string[] fileNames)
        {
            StreamReader sr;
            DataTable dtReceiveM = new DataTable();

            dtReceiveM.Columns.Add("ID");
            dtReceiveM.Columns.Add("BranchCode");
            dtReceiveM.Columns.Add("Receive_DateTime");
            dtReceiveM.Columns.Add("Reference_No");
            dtReceiveM.Columns.Add("Comments");
            dtReceiveM.Columns.Add("Post");
            dtReceiveM.Columns.Add("Return_Id");

            dtReceiveM.Columns.Add("Item_Code");
            dtReceiveM.Columns.Add("Item_Name");
            dtReceiveM.Columns.Add("Quantity");
            dtReceiveM.Columns.Add("NBR_Price");
            dtReceiveM.Columns.Add("UOM");
            dtReceiveM.Columns.Add("VAT_Name");
            dtReceiveM.Columns.Add("CustomerCode");


            var fileName = fileNames[0];

            sr = new StreamReader(fileName);
            string masterData = sr.ReadToEnd();
            string[] masterRows = masterData.Split("\r".ToCharArray());
            string delimeter = "|";

            foreach (string mRow in masterRows)
            {
                string[] mItems = mRow.Replace("\n", "").Split(delimeter.ToCharArray());
                dtReceiveM.Rows.Add(mItems);
            }

            if (sr != null)
            {
                sr.Close();
            }

            return dtReceiveM;
        }

        private DataTable GetPurchase(string[] fileNames)
        {
            StreamReader sr;
            DataTable dt;
            DataTable dtPurchaseM = new DataTable();


            dtPurchaseM.Columns.Add("ID");
            dtPurchaseM.Columns.Add("BranchCode");
            dtPurchaseM.Columns.Add("Vendor_Code");
            dtPurchaseM.Columns.Add("Vendor_Name");
            dtPurchaseM.Columns.Add("Referance_No");
            dtPurchaseM.Columns.Add("LC_No");
            dtPurchaseM.Columns.Add("BE_Number");
            dtPurchaseM.Columns.Add("Invoice_Date");
            dtPurchaseM.Columns.Add("Receive_Date");
            dtPurchaseM.Columns.Add("Post");
            dtPurchaseM.Columns.Add("With_VDS");
            dtPurchaseM.Columns.Add("Previous_Purchase_No");
            dtPurchaseM.Columns.Add("Comments");
            dtPurchaseM.Columns.Add("Custom_House");

            dtPurchaseM.Columns.Add("Item_Name");
            dtPurchaseM.Columns.Add("Item_Code");

            dtPurchaseM.Columns.Add("Quantity");
            dtPurchaseM.Columns.Add("Total_Price");
            dtPurchaseM.Columns.Add("UOM");
            dtPurchaseM.Columns.Add("Type");
            dtPurchaseM.Columns.Add("Rebate_Rate");
            dtPurchaseM.Columns.Add("SD_Amount");
            dtPurchaseM.Columns.Add("VAT_Amount");
            dtPurchaseM.Columns.Add("CnF_Amount");

            dtPurchaseM.Columns.Add("Insurance_Amount");
            dtPurchaseM.Columns.Add("Assessable_Value");
            dtPurchaseM.Columns.Add("CD_Amount");
            dtPurchaseM.Columns.Add("RD_Amount");
            dtPurchaseM.Columns.Add("TVB_Amount");
            dtPurchaseM.Columns.Add("TVA_Amount");
            dtPurchaseM.Columns.Add("ATV_Amount");
            dtPurchaseM.Columns.Add("Others_Amount");
            dtPurchaseM.Columns.Add("Remarks");

            var fileName = fileNames[0];

            sr = new StreamReader(fileName);
            string masterData = sr.ReadToEnd();
            string[] masterRows = masterData.Split("\r".ToCharArray());
            string delimeter = "|";

            foreach (string mRow in masterRows)
            {
                string[] mItems = mRow.Replace("\n", "").Split(delimeter.ToCharArray());
                dtPurchaseM.Rows.Add(mItems);
            }

            if (sr != null)
            {
                sr.Close();
            }

            dt = dtPurchaseM;
            return dt;
        }

        private void bgwTransferIssue_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                TransferIssueDAL issueDal = new TransferIssueDAL();

                SetPercentBar();

                var DataSet = new DataSet();

                DataSet.Tables.Add(dtTableResult);
                var xml = OrdinaryVATDesktop.GetXMLFromDataSet(DataSet);


                ////IntegrationParam param = new IntegrationParam();
                ////param.Data = dtTableResult;
                ////param.BranchCode = Program.BranchCode;
                ////param.TransactionType = transactionType;
                ////param.CurrentUser = Program.CurrentUser;
                ////param.DefaultBranchId = Program.BranchId;
                ////param.callBack = BulkCallBack;

                ////sqlResults = issueDal.SaveTransferIssue_Split(param);

                sqlResults = issueDal.SaveTempTransfer(dtTableResult, Program.BranchCode, transactionType,
                    Program.CurrentUser, Program.BranchId, BulkCallBack, null, null, Integration, connVM, "", Program.CurrentUserID);

                //////// SaveTransferIssue();
            }
            catch (Exception exception)
            {
                sqlResults[0] = "fail";
                sqlResults[1] = exception.Message;
                FileLogger.Log("FormMasterImport", "bgwTransferIssue", exception.Message);

            }
        }

        private void SaveTransferIssue()
        {
            #region Variables

            DataTable dtTransferM = new System.Data.DataTable();
            DataTable dtTransferD = new System.Data.DataTable();
            TransferIssueVM Master = new TransferIssueVM();
            List<TransferIssueDetailVM> Details = new List<TransferIssueDetailVM>();

            #endregion

            try
            {
                if (!dtTableResult.Columns.Contains("SerialNo"))
                {
                    var column = new DataColumn("SerialNo") { DefaultValue = "-" };
                    dtTableResult.Columns.Add(column);
                }

                DataView view = new DataView(dtTableResult);

                dtTransferM = view.ToTable(true, "ID", "TransferTo", "TransactionDateTime", "SerialNo", "ReferenceNo",
                    "TransactionType",
                    "Comments", "Post", "Branch_Code");
                dtTransferD = view.ToTable(true, "ID", "TransactionDateTime", "SerialNo", "ReferenceNo", "Comments",
                    "Post", "ProductCode", "ProductName", "Quantity", "UOM", "CommentsD");


                // dtTransferM = OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "TransactionType", transactionType, "string");
                dtTransferM = OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "CreatedBy", Program.CurrentUser, "string");
                dtTransferM =
                    OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "CreatedOn", DateTime.Now.ToString(), "string");
                dtTransferM =
                    OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "LastModifiedBy", Program.CurrentUser, "string");
                dtTransferM =
                    OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "LastModifiedOn", DateTime.Now.ToString(), "string");

                TransferIssueDAL trIssDal = new TransferIssueDAL();
                dtTransferM = OrdinaryVATDesktop.DtDateCheck(dtTransferM, new string[] { "TransactionDateTime" });
                dtTransferD = OrdinaryVATDesktop.DtDateCheck(dtTransferD, new string[] { "TransactionDateTime" });
                sqlResults = trIssDal.ImportData(dtTransferM, dtTransferD, Program.BranchId, null, null, null, false, connVM, Program.CurrentUserID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
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

        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                _timePassedInMs = 0;


                var salesDal = new SaleDAL();
                var salesData = dtTableResult.Copy();

                //salesData.Columns.Remove("VAT_Amount");
                //salesData.Columns.Remove("Group");

                var column = new DataColumn("SL") { DefaultValue = "" };
                salesData.Columns.Add(column);

                if (InvokeRequired)
                    Invoke((MethodInvoker)delegate { PercentBar(salesData.Rows.Count); });

                //sqlResults = salesDal.ImportBigData(salesData, true, BulkCallBack);
                //_noBranch = salesDal.SaleTempNoBranch();


                _timePassedInMs += Convert.ToInt64(sqlResults[2]);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BulkCallBack()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() => { progressBar1.Value += 1; }));

        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                // MessageBox.Show(this, (_timePassedInMs / 1000).ToString());
                //if (_saleRowCount == 0)
                //{
                //    MessageBox.Show(this, "No Unprocessed Data Left");
                //    return;
                //}

                if (_noBranch != null && _noBranch.Rows.Count > 0)
                {
                    var view = new DataView(_noBranch);

                    var temp = view.ToTable(false, "ID", "Customer_Name", "Customer_Code", "Delivery_Address", "Invoice_Date_Time",
                    "Delivery_Date_Time", "Item_Code", "Item_Name", "Quantity", "NBR_Price", "UOM", "Total_Price", "VAT_Rate", "Vehicle_No", "Reference_No",
                    "Comments", "Sale_Type", "Previous_Invoice_No", "Is_Print", "Tender_Id", "Post", "LC_Number", "Currency_Code", "SD_Rate", "Non_Stock", "Trading_MarkUp", "Type",
                    "Discount_Amount", "Promotional_Quantity", "VAT_Name", "ExpDescription", "ExpQuantity", "ExpGrossWeight", "ExpNetWeight",
                    "ExpNumberFrom", "ExpNumberTo", "Branch_Code");

                    dgvLoadedTable.DataSource = temp;
                    _isDeleteTemp = false;
                    MessageBox.Show(this, "For some data branch code does not exist. Please correct those.");
                }

                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    MessageBox.Show(this, "Import completed successfully");
                }
                else
                {
                    MessageBox.Show(this, "Import Failed");

                }

                loadedTable = CONST_SALETYPE;

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

        private void btnBigData_Click(object sender, EventArgs e)
        {
            try
            {

                //progressBar1.Visible = true;
                //BrowsFile();
                //if (preSelectTable == "Sales")
                //{
                //    string fileName = Program.ImportFileName;
                //    if (string.IsNullOrEmpty(fileName))
                //    {
                //        MessageBox.Show("Please select the right file for import");
                //        return;
                //    }

                //    #region Excel validation

                //    string[] extention = fileName.Split(".".ToCharArray());
                //    string[] retResults = new string[4];
                //    if (extention[extention.Length - 1] != "xls")
                //    {
                //        MessageBox.Show("You can select Excel(.xls) files only");
                //        return;
                //    }

                //    #endregion

                //    ds = LoadFromExcel();
                //    DataTable dt = ds.Tables["SaleM"];
                //    dgvLoadedTable.DataSource = dt;
                //    loadedTable = CONST_SALES_BIGDATA;

                //}
                //var dal = new CommonDAL();

                //var table = dtTableResult.Copy();

                //var tempList = table.ToList<SaleTempVM>();

                //dal.SaveTempTest(tempList);

                //var deposit = new FormDepositTDS();

                //deposit.ShowDialog();

                var saleImport = new FormSaleImport();

                saleImport.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                //progressBar1.Visible = false;

            }
        }

        private void btnUnprocessed_Click(object sender, EventArgs e)
        {

            var message = "Are you sure to process temp data?";
            var caption = "Sale temp data process";
            var buttons = MessageBoxButtons.YesNo;

            var result = MessageBox.Show(message, caption, buttons);

            if (result == DialogResult.No)
                return;

            if (result == DialogResult.Yes)
            {
                progressBar1.Visible = true;
                bgwSaveUnprocessed.RunWorkerAsync();
            }
        }

        private void bgwSaveUnprocessed_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _timePassedInMs = 0;
                SaveUnprocessed();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveUnprocessed()
        {
            try
            {

                var saleDal = new SaleDAL();
                var rowCount = saleDal.GetUnProcessedCount(connVM);
                _saleRowCount = rowCount;

                if (InvokeRequired)
                    Invoke((MethodInvoker)delegate { PercentBar(rowCount); });


                var successCount = 0;
                loadedTable = CONST_SALETYPE;
                selectedType = CONST_DATABASE;
                for (var i = 0; i < rowCount; i++)
                {
                    var id = saleDal.GetTopUnProcessed()[2];
                    dtTableResult = saleDal.GetById(id);
                    SaveSale();
                    _timePassedInMs += Convert.ToInt64(sqlResults[4]);
                    if (sqlResults[0] == null || sqlResults[0].ToLower() != "success") continue;
                    saleDal.UpdateIsProcessed(1, id);
                    successCount++;
                    if (InvokeRequired)
                        Invoke(new MethodInvoker(UpdateProgressBar));

                }

                sqlResults[0] = rowCount == successCount ? "success" : "fail";
            }
            catch (Exception e)
            {
                sqlResults[0] = "fail";
            }
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

        private void bgwSaveUnprocessed_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //_watch.Stop();
                // MessageBox.Show(this, (_timePassedInMs / 1000).ToString());
                if (_saleRowCount == 0)
                {
                    MessageBox.Show(this, "No Unprocessed Data Left");
                    return;
                }

                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    try
                    {
                        var result = new CommonDAL().UpdateIsVATComplete(loadedTable, null, null, connVM);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message);
                    }
                    MessageBox.Show(this, "Import completed successfully");
                }
                else
                {
                    MessageBox.Show(this, "Import Failed");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                progressBar1.Visible = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }


        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            if (loadedTable == "")
            {
                //SaveExcel_Audit();
                return;
            }

            if (!_isDeleteTemp)
            {
                MessageBox.Show(this, "Please select new excel file");
                return;
            }

            if (!IsRowSelected())
            {
                MessageBox.Show(this, "Please select at least one row!");
                return;
            }


            if (loadedTable == CONST_SALETYPE)
                ImportBigData();
        }

        private void bgwPurchaseBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _timePassedInMs = 0;

                var PurchaseDal = new PurchaseDAL();
                var PurchaseData = dtTableResult.Copy();

                var column = new DataColumn("SL") { DefaultValue = "" };
                PurchaseData.Columns.Add(column);

                sqlResults = PurchaseDal.ImportBigData(PurchaseData, connVM);
                _timePassedInMs += Convert.ToInt64(sqlResults[2]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bgwPurchaseBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                MessageBox.Show(this, (_timePassedInMs / 1000).ToString());

                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    MessageBox.Show(this, "Import completed successfully");
                }
                else
                {
                    MessageBox.Show(this, "Import Failed");

                }


                loadedTable = CONST_PURCHASETYPE;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void bgwIssueBigData_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                _timePassedInMs = 0;

                var issueDal = new IssueDAL();
                var issueData = dtTableResult.Copy();

                var column = new DataColumn("SL") { DefaultValue = "" };
                issueData.Columns.Add(column);

                sqlResults = issueDal.ImportBigData(issueData, connVM);
                _timePassedInMs += Convert.ToInt64(sqlResults[2]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bgwIssueBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(this, (_timePassedInMs / 1000).ToString());

                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    MessageBox.Show(this, "Import completed successfully");
                }
                else
                {
                    MessageBox.Show(this, "Import Failed");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void bgwReceiveBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _timePassedInMs = 0;

                var issueDal = new IssueDAL();
                var receiveData = dtTableResult.Copy();

                var column = new DataColumn("SL") { DefaultValue = "" };
                receiveData.Columns.Add(column);

                sqlResults = issueDal.ImportReceiveBigData(receiveData, connVM);
                _timePassedInMs += Convert.ToInt64(sqlResults[2]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bgwReceiveBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(this, (_timePassedInMs / 1000).ToString());

                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    MessageBox.Show(this, "Import completed successfully");
                }
                else
                {
                    MessageBox.Show(this, "Import Failed");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.progressBar1.Visible = false;
            }
        }

        private void bgwTransferReceive_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var issueData = dtTableResult.Copy();

                var dycryptedTable = new DataTable();

                foreach (DataColumn column in issueData.Columns)
                {
                    dycryptedTable.Columns.Add(column.ColumnName);
                }

                dycryptedTable.Columns.Add(new DataColumn("CurrentUser") { DefaultValue = Program.CurrentUser });
                dycryptedTable.Columns.Add(new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") });

                foreach (DataRow row in issueData.Rows)
                {

                    var items = row.ItemArray;

                    for (var i = 0; i < items.Length; i++)
                    {
                        items[i] = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, items[i].ToString());
                    }

                    dycryptedTable.Rows.Add(items);
                }

                foreach (DataRow row in dycryptedTable.Rows)
                {
                    if (row["TransferTo"].ToString().Trim() != Program.BranchId.ToString())
                    {
                        throw new Exception("This Branch is not the destination");
                    }
                }

                var dataView = new DataView(dycryptedTable);


                var master = dataView.ToTable(true, "TransferIssueNo", "IssueDateTime", "TotalAmount", "TransactionType",
                    "SerialNo", "Comments",
                    "Post", "ReferenceNo", "TransferTo", "BranchId", "IsTransfer", "BranchName", "BranchNameFrom", "TotalVATAmount", "TotalSDAmount", "CurrentUser", "CreatedOn");

                var details = dataView.ToTable(true, "TransferIssueNo", "ItemNo", "Quantity", "CostPrice", "UOM",
                    "SubTotal", "UOMQty", "UOMn", "UOMc", "UOMPrice", "VATRate", "VATAmount", "SDRate", "SDAmount", "ProductCode", "ProductName", "Stock", "BranchId");

                var dal = new TransferIssueDAL();

                foreach (DataRow row in master.Rows)
                {
                    row["TransactionType"] = transactionType;
                }

                sqlResults = dal.ImportTransferData(master, details, Program.BranchId, null, null, connVM);

            }
            catch (Exception exception)
            {
                sqlResults[1] = exception.Message;
                FileLogger.Log("MasterImport", "bgwTransferReceive_DoWork", exception.Message);
            }
        }

        private void bgwTransferReceive_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            }
        }

        private void cmbImportType_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (cmbImportType.Text == CONST_DATABASE && preSelectTable.ToLower() == CONST_TRANSFERTYPE.ToLower())
                {
                    string[] Condition = new string[] { "ActiveStatus='Y' AND BranchID NOT IN(" + Program.BranchId + ")" };
                    cmbTransferTo = new CommonDAL().ComboBoxLoad(cmbTransferTo, "BranchProfiles", "BranchID", "BranchCode", Condition, "varchar", true, false, connVM);

                    cmbTransferTo.Visible = true;
                }
                else
                {
                    cmbTransferTo.Visible = false;

                }

            }
            catch (Exception exception)
            {

            }
        }

        private void dgvLoadedTable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {

                string Id = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString();

                string refNo = "";
                if (dgvLoadedTable.Columns.Contains("ReferenceNo"))
                {
                    refNo = dgvLoadedTable.CurrentRow.Cells["ReferenceNo"].Value.ToString();
                }
                else if (dgvLoadedTable.Columns.Contains("Referance_No"))
                {
                    refNo = dgvLoadedTable.CurrentRow.Cells["Referance_No"].Value.ToString();

                }


                ////string refNo = dgvLoadedTable.CurrentRow.Cells["ReferenceNo"].Value != null ? dgvLoadedTable.CurrentRow.Cells["ReferenceNo"].Value.ToString() : "";


                _saleRow = string.IsNullOrEmpty(refNo) || refNo == "-" ? Id : refNo;

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void bgwExcellValidation_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                IsError = false;
                // List<ErrorMessage> errormessage = new List<ErrorMessage>();
                OrdinaryVATDesktop oDal = new OrdinaryVATDesktop();
                //  ReceiveDAL rDal = new ReceiveDAL();
                CommonDAL commonDal = new CommonDAL();
                string CustomeDateFormat = commonDal.settingValue("Integration", "CustomeDateFormat", connVM);


                if (loadedTable == CONST_RECEIVETYPE)
                {
                    ResultVM quantityResult = oDal.ExcelValidationNumeric(dtTableResult, "Quantity", "ID");
                    ResultVM receive_dateTime = oDal.ExcelValidationDateTime(dtTableResult, "Receive_DateTime", "ID");
                    ResultVM NBR_Price = oDal.ExcelValidationNumeric(dtTableResult, "NBR_Price", "ID");
                    ResultVM post = oDal.ExcelValidationYNCheck(dtTableResult, "Post", "ID");
                    ResultVM WithToll = oDal.ExcelValidationYNCheck(dtTableResult, "With_Toll", "ID");


                    errormessage.AddRange(quantityResult.ErrorList);
                    errormessage.AddRange(receive_dateTime.ErrorList);
                    errormessage.AddRange(NBR_Price.ErrorList);
                    errormessage.AddRange(post.ErrorList);
                    errormessage.AddRange(WithToll.ErrorList);

                }
                else if (loadedTable == CONST_PURCHASETYPE)
                {
                    ResultVM TotalPrice = oDal.ExcelValidationNumeric(dtTableResult, "Total_Price", "ID");
                    ResultVM Totalpost = oDal.ExcelValidationYNCheck(dtTableResult, "Post", "ID");
                    ResultVM TotalWith_VDS = oDal.ExcelValidationYNCheck(dtTableResult, "With_VDS", "ID");
                    ResultVM TotalQuantity = oDal.ExcelValidationNumeric(dtTableResult, "Quantity", "ID");
                    ResultVM TotalVAT_Amount = oDal.ExcelValidationNumeric(dtTableResult, "VAT_Amount", "ID");

                    ResultVM TotalAssessable_Value = null;
                    if (transactionType.ToLower() == "import")
                    {
                        TotalAssessable_Value = oDal.ExcelValidationNumeric(dtTableResult, "Assessable_Value", "ID");
                    }
                    ResultVM TotalIsRebate = oDal.ExcelValidationYNCheck(dtTableResult, "IsRebate", "ID");
                    ResultVM TotalType = oDal.ExcelValidationPTypeCheck(dtTableResult, "Type", "ID");

                    if (string.IsNullOrWhiteSpace(CustomeDateFormat) && CustomeDateFormat == "-")
                    {
                        ResultVM TInvoice_Date = oDal.ExcelValidationDateTime(dtTableResult, "Invoice_Date", "ID");
                        if (dtTableResult.Columns.Contains("Invoice_Time"))
                        {
                            ResultVM TInvoice_Time = oDal.ExcelValidationDateTime(dtTableResult, "Invoice_Time", "ID");
                            errormessage.AddRange(TInvoice_Time.ErrorList);

                        }

                        ResultVM TReceive_Date = oDal.ExcelValidationDateTime(dtTableResult, "Receive_Date", "ID");
                        if (dtTableResult.Columns.Contains("Receive_Time"))
                        {
                            ResultVM TReceive_Time = oDal.ExcelValidationDateTime(dtTableResult, "Receive_Time", "ID");
                            errormessage.AddRange(TReceive_Time.ErrorList);

                        }

                        errormessage.AddRange(TInvoice_Date.ErrorList);
                        errormessage.AddRange(TReceive_Date.ErrorList);

                    }

                    if (!dtTableResult.Columns.Contains("FixedVATRebate"))
                    {
                        System.Data.DataColumn newColumn = new System.Data.DataColumn("FixedVATRebate", typeof(System.String));
                        newColumn.DefaultValue = "N";
                        dtTableResult.Columns.Add(newColumn);
                    }

                    ResultVM FixedVATRebate = oDal.ExcelValidationYNCheck(dtTableResult, "FixedVATRebate", "ID");


                    errormessage.AddRange(TotalPrice.ErrorList);
                    errormessage.AddRange(Totalpost.ErrorList);
                    errormessage.AddRange(TotalWith_VDS.ErrorList);
                    errormessage.AddRange(TotalQuantity.ErrorList);
                    errormessage.AddRange(TotalVAT_Amount.ErrorList);
                    if (transactionType.ToLower() == "import")
                    {
                        errormessage.AddRange(TotalAssessable_Value.ErrorList);
                    }
                    errormessage.AddRange(TotalIsRebate.ErrorList);
                    errormessage.AddRange(TotalType.ErrorList);
                    //errormessage.AddRange(TInvoice_Date.ErrorList);
                    //errormessage.AddRange(TReceive_Date.ErrorList);
                    errormessage.AddRange(FixedVATRebate.ErrorList);

                }
                else if (loadedTable == CONST_VDSTYPE)
                {
                    ResultVM TEffect_Date = oDal.ExcelValidationDateTime(dtTableResult, "Effect_Date", "ID");
                    ResultVM TBankDepositDate = oDal.ExcelValidationDateTime(dtTableResult, "BankDepositDate", "ID");
                    ResultVM TPost = oDal.ExcelValidationYNCheck(dtTableResult, "Post", "ID");
                    ResultVM TBill_Amount = oDal.ExcelValidationNumeric(dtTableResult, "Bill_Amount", "ID");
                    ResultVM TBill_Date = oDal.ExcelValidationDateTime(dtTableResult, "Bill_Date", "ID");
                    ResultVM TVDS_Amount = oDal.ExcelValidationNumeric(dtTableResult, "VDS_Amount", "ID");
                    ResultVM TIssue_Date = oDal.ExcelValidationDateTime(dtTableResult, "Issue_Date", "ID");
                    ResultVM TIsPurchase = oDal.ExcelValidationYNCheck(dtTableResult, "IsPurchase", "ID");
                    ResultVM TCheque_Date = oDal.ExcelValidationDateTime(dtTableResult, "Cheque_Date", "ID");

                    errormessage.AddRange(TEffect_Date.ErrorList);
                    errormessage.AddRange(TBankDepositDate.ErrorList);
                    errormessage.AddRange(TPost.ErrorList);
                    errormessage.AddRange(TBill_Amount.ErrorList);
                    errormessage.AddRange(TBill_Date.ErrorList);
                    errormessage.AddRange(TVDS_Amount.ErrorList);
                    errormessage.AddRange(TIssue_Date.ErrorList);
                    errormessage.AddRange(TIsPurchase.ErrorList);
                    errormessage.AddRange(TCheque_Date.ErrorList);


                }
                else if (loadedTable == CONST_BOMTYPE)
                {
                    string singleImport = new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "BOM");
                    singleImport = "Y";
                    if (singleImport.ToLower() == "y")
                    {
                        ResultVM TFirstSupplyDate = oDal.ExcelValidationDateTime(dtTableResult, "FirstSupplyDate", "FCode");
                        ResultVM TEffectDate = oDal.ExcelValidationDateTime(dtTableResult, "EffectDate", "FCode");
                        ResultVM totalQuantity = oDal.ExcelValidationNumeric(dtTableResult, "TotalQuantity", "FCode");
                        ResultVM TUseQuantity = oDal.ExcelValidationNumeric(dtTableResult, "UseQuantity", "FCode");
                        ResultVM wastageQuantity = oDal.ExcelValidationNumeric(dtTableResult, "WastageQuantity", "FCode");
                        ResultVM TCost = oDal.ExcelValidationNumeric(dtTableResult, "Cost", "FCode");
                        ResultVM issueOnProduction = oDal.ExcelValidationYNCheck(dtTableResult, "IssueOnProduction", "FCode");
                        ResultVM TRebateRate = oDal.ExcelValidationNumeric(dtTableResult, "RebateRate", "FCode");
                        ResultVM TVatname = oDal.ExcelValidationVatnameColumnCheck(dtTableResult, "VATName", "FCode");

                        errormessage.AddRange(TFirstSupplyDate.ErrorList);
                        errormessage.AddRange(TEffectDate.ErrorList);
                        errormessage.AddRange(totalQuantity.ErrorList);
                        errormessage.AddRange(TUseQuantity.ErrorList);
                        errormessage.AddRange(wastageQuantity.ErrorList);
                        errormessage.AddRange(TCost.ErrorList);
                        errormessage.AddRange(issueOnProduction.ErrorList);
                        errormessage.AddRange(TRebateRate.ErrorList);
                        errormessage.AddRange(TVatname.ErrorList);

                    }
                    else
                    {
                        ResultVM TFirstSupplyDate = oDal.ExcelValidationDateTime(dtTableResult, "FirstSupplyDate", "P-Code");
                        ResultVM TEffectDate = oDal.ExcelValidationDateTime(dtTableResult, "EffectDate", "P-Code");
                        ResultVM SDRate = oDal.ExcelValidationNumeric(dtTableResult, "SDRate", "P-Code");
                        ResultVM VATRate = oDal.ExcelValidationNumeric(dtTableResult, "VATRate", "P-Code");
                        ResultVM Margin = oDal.ExcelValidationNumeric(dtTableResult, "Margin", "P-Code");
                        ResultVM PacketPrice = oDal.ExcelValidationNumeric(dtTableResult, "PacketPrice", "P-Code");
                        ResultVM TradingMarkup = oDal.ExcelValidationNumeric(dtTableResult, "TradingMarkup", "P-Code");
                        ResultVM PreviousSDApplyedPrice = oDal.ExcelValidationNumeric(dtTableResult, "PreviousSDApplyedPrice", "P-Code");
                        ResultVM PreviousVATABLE_PRICE = oDal.ExcelValidationNumeric(dtTableResult, "PreviousVATABLE PRICE", "P-Code");
                        ResultVM VATABLE_PRICE = oDal.ExcelValidationNumeric(dtTableResult, "VATABLE PRICE", "P-Code");
                        errormessage.AddRange(TFirstSupplyDate.ErrorList);
                        errormessage.AddRange(TEffectDate.ErrorList);
                        errormessage.AddRange(SDRate.ErrorList);
                        errormessage.AddRange(VATRate.ErrorList);
                        errormessage.AddRange(Margin.ErrorList);
                        errormessage.AddRange(PacketPrice.ErrorList);
                        errormessage.AddRange(TradingMarkup.ErrorList);
                        errormessage.AddRange(PreviousSDApplyedPrice.ErrorList);
                        errormessage.AddRange(PreviousVATABLE_PRICE.ErrorList);
                        errormessage.AddRange(VATABLE_PRICE.ErrorList);
                    }

                }


                // return IsError;

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "bgwExcellValidation_DoWork", ex.ToString());
                throw;
            }


        }

        private void bgwExcellValidation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (errormessage.Count > 0)
                {
                    FormErrorMessage.ShowDetails(errormessage);
                    IsError = true;
                }

                errormessage.Clear();

                if (!IsError)
                {
                    dgvLoadedTable.DataSource = OrdinaryVATDesktop.CopyTableInString(dtTableResult);

                }

                //loadedTable = CONST_RECEIVETYPE;
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "bgwExcellValidation_RunWorkerCompleted", ex.ToString());
                throw;
            }
            finally
            {
                this.progressBar1.Visible = false;

            }
        }

        private void bgwIssueExcellValidation_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                IsError = false;
                // List<ErrorMessage> errormessage = new List<ErrorMessage>();
                // OrdinaryVATDesktop oDal = new OrdinaryVATDesktop();
                IssueDAL iDal = new IssueDAL();

                ResultVM quantityResult = iDal.ExcelReceiveValidation(dtTableResult, "Quantity");
                //ResultVM receive_dateTime = iDal.ExcelReceiveValidation(dtTableResult, "Issue_DateTime");
                ResultVM post = iDal.ExcelReceiveValidation(dtTableResult, "Post");

                errormessage.AddRange(quantityResult.ErrorList);
                //errormessage.AddRange(receive_dateTime.ErrorList);             
                errormessage.AddRange(post.ErrorList);



                // return IsError;

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "bgwIssueExcellValidation_DoWork", ex.ToString());
                throw;
            }


        }

        private void bgwIssueExcellValidation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (errormessage.Count > 0)
                {
                    FormErrorMessage.ShowDetails(errormessage);

                    IsError = true;
                }

                errormessage.Clear();

                if (!IsError)
                {
                    dgvLoadedTable.DataSource = OrdinaryVATDesktop.CopyTableInString(dtTableResult);

                }

                loadedTable = CONST_ISSUETYPE;
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "bgwIssueExcellValidation_RunWorkerCompleted", ex.ToString());
                throw;
            }
            finally
            {
                this.progressBar1.Visible = false;

            }

        }

        private void bgwTransferIssueExcellValidation_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                IsError = false;
                // List<ErrorMessage> errormessage = new List<ErrorMessage>();

                IssueDAL iDal = new IssueDAL();

                ResultVM quantityResult = iDal.ExcelReceiveValidationRM(dtTableResult, "Quantity");
                ResultVM vat_rate = iDal.ExcelReceiveValidationRM(dtTableResult, "VAT_Rate");
                ResultVM costprice = iDal.ExcelReceiveValidationRM(dtTableResult, "CostPrice");
                ResultVM post = iDal.ExcelReceiveValidationRM(dtTableResult, "Post");

                errormessage.AddRange(quantityResult.ErrorList);
                errormessage.AddRange(vat_rate.ErrorList);
                errormessage.AddRange(costprice.ErrorList);
                errormessage.AddRange(post.ErrorList);



                // return IsError;

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "bgwTransferIssueExcellValidation_DoWork", ex.ToString());
                throw;
            }


        }

        private void bgwTransferIssueExcellValidation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (errormessage.Count > 0)
                {
                    FormErrorMessage.ShowDetails(errormessage);

                    IsError = true;
                }

                errormessage.Clear();

                if (!IsError)
                {
                    dgvLoadedTable.DataSource = OrdinaryVATDesktop.CopyTableInString(dtTableResult);

                }

                loadedTable = CONST_TRANSFERTYPE;
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "bgwTransferIssueExcellValidation_RunWorkerCompleted", ex.ToString());
                throw;
            }
            finally
            {
                this.progressBar1.Visible = false;

            }

        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


    }
}
