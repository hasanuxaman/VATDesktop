using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;
//using Microsoft.SqlServer.Management.Common;

using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VMSAPI;
using System.Text;


namespace VATClient
{
    public partial class FormSaleImportBSL : Form
    {
        #region Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        string[] sqlResults = new string[6];
        string selectedTable;
        string loadedTable;
        static string selectedType;
        private int _saleRowCount = 0;
        DataTable dtTableResult;
        DataSet ds;
        string Transaction_Type = null;
        public bool isFileSelected = false;
        private bool _isDeleteTemp = true;
        public string preSelectTable;
        public string transactionType;
        private DataTable _noBranch;
        private long _timePassedInMs;
        private const string CONST_DATABASE = "Database";

        private const string CONST_SUN = "SUN";

        private const string CONST_TEXT = "Text";
        private const string CONST_EXCEL = "Excel";
        private const string CONST_XML = "XML";
        private const string CONST_ProcessOnly = "ProcessOnly";
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

        private string _saleRow = "";

        private string _selectedDb = "Link3_Demo_DB";

        public bool IsCDN = false;
        private bool IsError = false;
        List<ErrorMessage> errormessage = new List<ErrorMessage>();

        #endregion

        public FormSaleImportBSL()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        public static string SelectOne(string transactionType, bool cdn)
        {
            FormSaleImportBSL form = new FormSaleImportBSL();

            form.IsCDN = cdn;

            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.Show();

            return form._saleRow;
        }

        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            dgvLoadedTable.ReadOnly = true;
            tableLoad();
            typeLoad();
        }

        private void tableLoad()
        {
            if (transactionType == "TollIssue")
            {
                this.Text = "Toll Issue Import";
            }
            else if (transactionType == "TollFinishIssue")
            {
                this.Text = "Toll Finish Issue";
            }
        }

        private void typeLoad()
        {
            cmbImportType.Items.Add(CONST_SUN);
            cmbImportType.Items.Add(CONST_TEXT);
            CommonDAL dal = new CommonDAL();

            string value = dal.settingsDesktop("Import", "SaleImportSelection", null, connVM);

            cmbImportType.SelectedItem = CONST_SUN;

            cmbImportType.Text = value;

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (cmbImportType.Text != CONST_DATABASE)
                {
                    _isDeleteTemp = true;
                    btnSaveTemp.Enabled = true;
                    LoadDataGrid();
                }
                if (dtTableResult != null)
                {
                    lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;
                }
            }
            catch (Exception ex)
            {
                dtTableResult = new DataTable();
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Program.ImportFileName = null;
                progressBar1.Visible = false;
            }
        }

        private void LoadDataGrid()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Program.ImportFileName))
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

                #region SUN Validation
                if (selectedType == CONST_SUN)
                {
                    string[] extention = fileName.Split(".".ToCharArray());
                    string[] retResults = new string[4];
                    if (extention[extention.Length - 1] == "SUN")
                    {
                        //
                    }
                    else
                    {
                        MessageBox.Show(this, "You can select SUN(.SUN) files only");
                        return;
                    }
                }
                #endregion

                progressBar1.Visible = true;

                selectedType = cmbImportType.Text;

                #region SUN
                if (selectedType == CONST_SUN)
                {
                    BSLIntegrationDAL dal = new BSLIntegrationDAL();
                    var result = dal.ReadAttachedFile(Program.ImportFileName);
                    dtTableResult = dal.ConvertListToDataTable(result);
                    dal.AddNewSalesColumn(dtTableResult);
                    dtTableResult = dal.GetSaleData(dtTableResult, null, null, connVM);
                    dtTableResult = dal.UpdateUnitPrice(dtTableResult);
                    loadedTable = CONST_SALETYPE;
                    dgvLoadedTable.DataSource = dtTableResult;
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void BrowsFile()
        {
            #region try
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                if (cmbImportType.Text == CONST_EXCEL)
                {
                    //fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                    //BugsBD
                    fdlg.Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (*.xlsm)|*.xlsm|Excel(97-2003) files (*.xls)|*.xls|Text files (*.txt)|*.txt";
                }
                else if (cmbImportType.Text == CONST_XML)
                {

                    fdlg.Filter = "XML Files (*.xml)|*.xml";
                  
                }
                else if (cmbImportType.Text == CONST_SUN)
                {
                    fdlg.Filter = "SUN files (*.SUN*)|*.SUN*";
                }
                else
                {
                    fdlg.Filter = "Text files (*.txt*)|*.txt*";
                }

                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                fdlg.Multiselect = true;
                int count = 0;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
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
                Program.ImportFileName = null;
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

                       

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            ResultVM rVM = new ResultVM();
            SaleDAL salesDal = new SaleDAL();
            IntegrationParam paramVM = new IntegrationParam();

            try
            {
                if (dtTableResult.Rows.Count > 0)
                {
                    if (!dtTableResult.Columns.Contains("TransactionType"))
                    {
                        var TransactionType = new DataColumn("TransactionType") { DefaultValue = "ServiceNS" };
                        dtTableResult.Columns.Add(TransactionType);
                    }

                    string[] result = salesDal.SaveAndProcess(dtTableResult, () => { }, Convert.ToInt32(Program.BranchId), "", connVM, paramVM, null, null, Program.CurrentUserID);

                    rVM.Status = result[0];
                    rVM.Message = result[1];

                    MessageBox.Show(this, rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(this, "No Data Found!");
                    return;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        


        #region Save Portion
        private bool IsRowSelected()
        {
            try
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
            catch (Exception ex)
            {
                throw;
            }
        }

        private void backgroundSaveSale_DoWork(object sender, DoWorkEventArgs e)
        {
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
                MessageBox.Show(this, "Import completed successfully");
            }
            else
            {
                MessageBox.Show(this, "Import Failed");
            }
        }

        #endregion

        #region Others

        private void ImportBigData()
        {

            //var saleDal = new SaleDAL();
            //if (saleDal.GetUnProcessedCount() > 0 && _isDeleteTemp)
            //{
            //    var dialogResult = MessageBox.Show(this,
            //        @"There are still unprocessed data remains. All those data will be deleted.", "Are you sure?",
            //        MessageBoxButtons.YesNo);
            //    if (dialogResult != DialogResult.Yes) return;
            //}
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

        private DataTable GetTableFromTextDouble()
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

            DataTable dtSaleM = new DataTable();
            DataTable dtSaleD = new DataTable();
            DataTable dtSaleE = new DataTable();

            try
            {
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
                dtSaleD.Columns.Add("SubTotal");
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
                // dt = dtSaleM;

                dt = PopulateColumn(dt);
                var j = 0;
                foreach (DataRow row in dtSaleM.Rows)
                {
                    var details = dtSaleD.Select("ID=" + row["ID"]).CopyToDataTable();

                    foreach (DataRow detail in details.Rows)
                    {
                        dt.Rows.Add(dt.NewRow());

                        var columnsCount = dt.Columns.Count;

                        for (var i = 0; i < columnsCount; i++)
                        {
                            var columnName = dt.Columns[i].ColumnName;

                            if (dtSaleM.Columns.Contains(columnName))
                            {
                                dt.Rows[j][columnName] = row[columnName];

                            }
                            else if (dtSaleD.Columns.Contains(columnName))
                            {
                                if (columnName == "SubTotal" && string.IsNullOrEmpty(detail[columnName].ToString()))
                                {
                                    dt.Rows[j][columnName] = "0";
                                }
                                else
                                {
                                    dt.Rows[j][columnName] = detail[columnName];

                                }

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


            return dt;
        }

        private DataTable PopulateColumn(DataTable table)
        {
            table.Rows.Clear();
            table.Columns.Clear();


            table.Columns.Add("ID");
            table.Columns.Add("Branch_Code");
            table.Columns.Add("CustomerGroup");
            table.Columns.Add("Customer_Name");

            table.Columns.Add("Customer_Code");
            table.Columns.Add("Delivery_Address");
            table.Columns.Add("Invoice_Date_Time");
            table.Columns.Add("Delivery_Date_Time");
            table.Columns.Add("Reference_No");
            table.Columns.Add("Comments");
            table.Columns.Add("Sale_Type");
            table.Columns.Add("Previous_Invoice_No");
            table.Columns.Add("Is_Print");
            table.Columns.Add("Tender_Id");
            table.Columns.Add("Post");
            table.Columns.Add("LC_Number");
            table.Columns.Add("Currency_Code");
            table.Columns.Add("CommentsD");
            table.Columns.Add("Item_Code");
            table.Columns.Add("Item_Name");

            table.Columns.Add("Quantity");
            table.Columns.Add("NBR_Price");
            table.Columns.Add("UOM");
            table.Columns.Add("VAT_Rate");
            table.Columns.Add("SD_Rate");
            table.Columns.Add("Non_Stock");
            table.Columns.Add("Trading_MarkUp");
            table.Columns.Add("Type");
            table.Columns.Add("Discount_Amount");
            table.Columns.Add("Promotional_Quantity");
            table.Columns.Add("VAT_Name");
            table.Columns.Add("SubTotal");

            table.Columns.Add("Vehicle_No");



            table.Columns.Add("ExpDescription");

            table.Columns.Add("ExpQuantity");

            table.Columns.Add("ExpGrossWeight");
            table.Columns.Add("ExpNetWeight");
            table.Columns.Add("ExpNumberFrom");
            table.Columns.Add("ExpNumberTo");

            return table;
        }

        private void chkSelectAll_Click(object sender, EventArgs e)
        {
            //SelectAll();
        }

        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                var salesDal = new SaleDAL();

                if (selectedType == CONST_ProcessOnly)
                {
                    sqlResults = salesDal.SaveAndProcessTempData(new DataTable(), BulkCallBack, Program.BranchId, connVM);
                }
                else
                {
                    DataTable salesData = dtTableResult.Copy();
                    //salesData = OrdinaryVATDesktop.FormatSaleData(salesData);

                    OrdinaryVATDesktop.DtDeleteColumn(salesData, "SubToalDiff");

                    TableValidation(salesData);

                    if (InvokeRequired)
                        Invoke((MethodInvoker)delegate { PercentBar(5); });



                    BulkCallBack();

                    var saleAPI = new SaleAPI();

                    var saleDataSet = new DataSet();

                    #region Comments


                    #endregion

                    var value = new CommonDAL().settingsDesktop("CompanyCode", "Code", null, connVM);

                    if (value.ToLower() == "SSDTech".ToLower() && transactionType.ToLower() == "ServiceNS".ToLower())
                    {
                        sqlResults = salesDal.SaveAndProcessWithBulkCustomer(salesData, BulkCallBack, Program.BranchId, Program.GetAppVersion(), connVM, null, null, null, Program.CurrentUserID);
                    }
                    else if (transactionType.ToLower() == "other" || transactionType.ToLower() == "credit" || transactionType.ToLower() == "debit" || transactionType.ToLower() == "ServiceNS".ToLower() || transactionType.ToLower() == "Export".ToLower())
                    {

                        //------------------------------------------------------------------------------------------------------------------------
                        sqlResults = salesDal.SaveAndProcess(salesData, BulkCallBack, Program.BranchId, Program.GetAppVersion(), connVM, null, null, null, Program.CurrentUserID);

                    }
                    else
                    {
                        sqlResults = salesDal.SaveAndProcess_WithOutBulk(salesData, BulkCallBack, Program.BranchId,
                            Program.GetAppVersion(), connVM);
                    }
                }



            }
            catch (Exception ex)
            {
                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
        }

        private void UpdateOtherDB(SaleDAL salesDal)
        {
            if (sqlResults[0].ToLower() == "success" && selectedType == CONST_DATABASE && _selectedDb == "Link3_Demo_DB")
            {
                var table = salesDal.GetInvoiceNoFromTemp();

                var results = salesDal.SaveSalesToLink3(table, connVM);

                if (results[0].ToLower() != "success")
                {
                    var message = "These Id and SalesInvoiceNo failed to insert to other database\n";

                    foreach (DataRow row in table.Rows)
                    {
                        message += row["ID"] + "-" + row["SalesInvoiceNo"] + "\n";
                    }

                    FileLogger.Log("FormSaleImportBSL", "bgwBigData_DoWork", message);
                }
            }
        }

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
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };
            DataColumn TransactionType = new DataColumn("TransactionType") { DefaultValue = transactionType };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }
            if (!salesData.Columns.Contains("TransactionType"))
            {
                salesData.Columns.Add(TransactionType);
            }
            salesData.Columns.Add(BOMId);

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
                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    MessageBox.Show(this, "Import completed successfully");
                }
                else
                {
                    MessageBox.Show(this, sqlResults[1]);

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
                var deposit = new FormDepositTDS();

                deposit.ShowDialog();

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

                //if (InvokeRequired)
                //    Invoke((MethodInvoker)delegate { PercentBar(rowCount); });


                var successCount = 0;
                loadedTable = CONST_SALETYPE;
                selectedType = CONST_DATABASE;

                #region Previous

                //for (var i = 0; i < rowCount; i++)
                //{
                //    var id = saleDal.GetTopUnProcessed()[2];
                //    dtTableResult = saleDal.GetById(id);
                //    SaveSale();
                //    _timePassedInMs += Convert.ToInt64(sqlResults[4]);
                //    if (sqlResults[0] == null || sqlResults[0].ToLower() != "success") continue;
                //    saleDal.UpdateIsProcessed(1, id);
                //    successCount++;
                //    if (InvokeRequired)
                //        Invoke(new MethodInvoker(UpdateProgressBar));

                //}

                //var masters = saleDal.GetMasterData();



                //var view = new DataView(masters);

                //masters = view.ToTable(true, "ID", "Customer_Name", "Customer_Code", "Delivery_Address",
                //    "Branch_Code", "Vehicle_No",
                //    "Invoice_Date_Time", "Delivery_Date_Time", "Reference_No", "Comments", "Sale_Type",
                //    "Previous_Invoice_No", "Is_Print", "Tender_Id",
                //    "Post", "LC_Number", "Currency_Code", "CurrencyID", "CustomerID", "BranchId", "VehicleID");


                //var masterVMs = new List<SaleMasterVM>();

                //foreach (DataRow master in masters.Rows)
                //{
                //    var vm = new SaleMasterVM();

                //    vm.SalesInvoiceNo = master["ID"].ToString();
                //    vm.CustomerID = master["CustomerID"].ToString();
                //    vm.VehicleID = master["VehicleID"].ToString();

                //    masterVMs.Add(vm);
                //}



                //sqlResults = saleDal.ImportSalesBigData(masters);

                //sqlResults[0] = rowCount == successCount ? "success" : "fail";

                #endregion


                //var invoiceId = saleDal.GetFisrtInvoiceId();

                //var id = Convert.ToInt32(invoiceId[2].Split('-')[1].Split('/')[0]);

                //sqlResults = saleDal.SaveInvoiceIdSaleTemp(id);

                //var masters = saleDal.GetMasterData();
                //sqlResults = saleDal.ImportSalesBigData(masters);

                //saleDal.SaveAndProcess()
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

        private void SaveExcel_Audit()
        {

            if (dtTableResult != null && dtTableResult.Rows.Count != 0)
            {

                string sourceFile = Program.ImportFileName;

                OrdinaryVATDesktop.SaveExcel_Audit(sourceFile);

            }

        }        

        private void dgvLoadedTable_DoubleClick(object sender, EventArgs e)
        {

        }

        private void dgvLoadedTable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _saleRow = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString();

            this.Hide();


        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void bgwSaleExcellvaidation_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                IsError = false;
                // ReceiveDAL rDal = new ReceiveDAL();
                OrdinaryVATDesktop oDal = new OrdinaryVATDesktop();

                if (loadedTable == CONST_SALETYPE)
                {
                    #region Erorr Check

                    ResultVM TotalQuantity = oDal.ExcelValidationNumeric(dtTableResult, "Quantity", "ID");
                    ResultVM TNBR_Price = oDal.ExcelValidationNumeric(dtTableResult, "NBR_Price", "ID");
                    ResultVM ZNBR_Price = oDal.ExcelValidationZeroCheck(dtTableResult, "NBR_Price", "ID");
                    ResultVM TSubTotal = oDal.ExcelValidationNumeric(dtTableResult, "SubTotal", "ID");
                    ResultVM ZSubTotal = oDal.ExcelValidationZeroCheck(dtTableResult, "SubTotal", "ID");
                    ResultVM TIs_Print = oDal.ExcelValidationYNCheck(dtTableResult, "Is_Print", "ID");
                    ResultVM TPost = oDal.ExcelValidationYNCheck(dtTableResult, "Post", "ID");
                    ResultVM TNon_Stock = oDal.ExcelValidationYNCheck(dtTableResult, "Non_Stock", "ID");
                    ResultVM TVAT_Rate = oDal.ExcelValidationNumeric(dtTableResult, "VAT_Rate", "ID");
                    ResultVM TInvoice_Date = oDal.ExcelValidationDateTime(dtTableResult, "PreviousInvoiceDateTime", "ID");
                    ResultVM TVAT_Amount = oDal.ExcelValidationNumeric(dtTableResult, "VAT_Amount", "ID");
                    ResultVM TSD_Rate = oDal.ExcelValidationNumeric(dtTableResult, "SD_Rate", "ID");
                    ResultVM TSDAmount = oDal.ExcelValidationNumeric(dtTableResult, "SDAmount", "ID");
                    ResultVM TDiscount_Amount = oDal.ExcelValidationNumeric(dtTableResult, "Discount_Amount", "ID");
                    ResultVM TPromotional_Quantity = oDal.ExcelValidationNumeric(dtTableResult, "Promotional_Quantity", "ID");
                    ResultVM TPreviousNBRPrice = oDal.ExcelValidationNumeric(dtTableResult, "PreviousNBRPrice", "ID");
                    ResultVM TPreviousQuantity = oDal.ExcelValidationNumeric(dtTableResult, "PreviousQuantity", "ID");
                    //ResultVM TPreviousSubTotal = oDal.ExcelValidationNumeric(dtTableResult, "PreviousSubTotal", "ID");
                    //ResultVM TPreviousSDAmount = oDal.ExcelValidationNumeric(dtTableResult, "PreviousSDAmount", "ID");
                    ResultVM TPreviousVATRate = oDal.ExcelValidationNumeric(dtTableResult, "PreviousVATRate", "ID");
                    //ResultVM TPreviousVATAmount = oDal.ExcelValidationNumeric(dtTableResult, "PreviousVATAmount", "ID");
                    ResultVM TPreviousSD = oDal.ExcelValidationNumeric(dtTableResult, "PreviousSD", "ID");
                    ResultVM TType = oDal.ExcelValidationSTypeCheck(dtTableResult, "Type", "ID");


                    #endregion

                    #region Add erorr message

                    errormessage.AddRange(TotalQuantity.ErrorList);
                    errormessage.AddRange(TNBR_Price.ErrorList);
                    errormessage.AddRange(ZNBR_Price.ErrorList);
                    errormessage.AddRange(TSubTotal.ErrorList);
                    errormessage.AddRange(ZSubTotal.ErrorList);
                    errormessage.AddRange(TIs_Print.ErrorList);
                    errormessage.AddRange(TPost.ErrorList);
                    errormessage.AddRange(TNon_Stock.ErrorList);
                    errormessage.AddRange(TVAT_Rate.ErrorList);
                    errormessage.AddRange(TVAT_Amount.ErrorList);
                    //errormessage.AddRange(TInvoice_Date.ErrorList);
                    errormessage.AddRange(TSD_Rate.ErrorList);
                    errormessage.AddRange(TSDAmount.ErrorList);
                    errormessage.AddRange(TDiscount_Amount.ErrorList);
                    errormessage.AddRange(TPromotional_Quantity.ErrorList);
                    errormessage.AddRange(TPreviousNBRPrice.ErrorList);
                    errormessage.AddRange(TPreviousQuantity.ErrorList);
                    //errormessage.AddRange(TPreviousSubTotal.ErrorList);
                    //errormessage.AddRange(TPreviousSDAmount.ErrorList);
                    errormessage.AddRange(TPreviousVATRate.ErrorList);
                    //errormessage.AddRange(TPreviousVATAmount.ErrorList);
                    errormessage.AddRange(TPreviousSD.ErrorList);
                    errormessage.AddRange(TType.ErrorList);
                    errormessage.AddRange(TInvoice_Date.ErrorList);


                    #endregion

                }


                // return IsError;

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "bgwSaleExcellvaidation_DoWork", ex.ToString());
                throw;
            }

        }

        private void bgwSaleExcellvaidation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                    #region Showing SubTotal Difference

                    DataColumn dataColumn = new DataColumn
                    {
                        ColumnName = "SubToalDiff",
                        DefaultValue = 0,
                        DataType = typeof(decimal)
                    };

                    dtTableResult.Columns.Add(dataColumn);
                    int rowsCount = dtTableResult.Rows.Count;

                    for (int i = 0; i < rowsCount; i++)
                    {
                        decimal quantity = Convert.ToDecimal(dtTableResult.Rows[i]["Quantity"].ToString());
                        decimal nbr_price = Convert.ToDecimal(dtTableResult.Rows[i]["NBR_Price"].ToString());
                        decimal subTotal = Convert.ToDecimal(dtTableResult.Rows[i]["SubTotal"].ToString());

                        decimal diff = subTotal - (quantity * nbr_price);

                        dtTableResult.Rows[i]["SubToalDiff"] = diff;
                    }

                    #region TransactionType Cloumn Check

                    if (selectedType != CONST_TEXT)
                    {
                        if (!dtTableResult.Columns.Contains("TransactionType"))
                        {
                            MessageBox.Show(this, "TransactionType Column Required in Excel Template", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else
                        {
                            DataView view = new DataView(dtTableResult);
                            DataTable distinctValues = view.ToTable(true, "TransactionType");
                            if (distinctValues.Rows.Count > 1)
                            {
                                MessageBox.Show(this, "There Multiple TransactionType Value in Excel Template", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            else
                            {
                                Transaction_Type = distinctValues.Rows[0]["TransactionType"].ToString();

                                if (Transaction_Type != transactionType)
                                {
                                    MessageBox.Show(this, "This Excel Template Can’t be upload here only Upload TransactionType '" + transactionType + " ' Data", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                            }
                        }
                    }
                    #endregion

                    dgvLoadedTable.DataSource = dtTableResult;
                    dgvLoadedTable.Columns["SubToalDiff"].DefaultCellStyle.ForeColor = Color.GreenYellow;
                    dgvLoadedTable.Columns["SubToalDiff"].DefaultCellStyle.BackColor = Color.DarkRed;
                    dgvLoadedTable.Columns["SubToalDiff"].DisplayIndex = 14;

                    #endregion

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "bgwSaleExcellvaidation_RunWorkerCompleted", ex.ToString());
                throw;
            }
            finally
            {
                this.progressBar1.Visible = false;

            }

        }

        #endregion



    }
}