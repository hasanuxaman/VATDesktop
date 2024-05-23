using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
//using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using Microsoft.SqlServer.Management.Common;
using SymphonySofttech.Reports;
using VATClient.ReportPreview;

namespace VATClient
{
    public partial class FormSaleImportACI : Form
    {
        #region Variables
        string[] sqlResults = new string[6];
        string selectedTable;
        string loadedTable;
        static string selectedType;
        private int _saleRowCount = 0;
        DataTable dtTableResult = new DataTable();
        DataSet ds;
        private bool _isDeleteTemp = true;
        public string preSelectTable;
        public string transactionType;
        private DataTable _noBranch;
        private long _timePassedInMs;
        private const string CONST_DATABASE = "Database";
        private const string CONST_TEXT = "Text";
        private const string CONST_EXCEL = "Excel";
        private const string CONST_ProcessOnly = "ProcessOnly";
        private const string CONST_SINGLEIMPORT = "SingleFileImport";
        private const string CONST_SALETYPE = "Sales";
        private const string CONST_PURCHASETYPE = "Purchases";
        private const string CONST_ISSUETYPE = "Issues";
        private const string CONST_RECEIVETYPE = "Receives";
        private const string CONST_VDSTYPE = "VDS";//
        private const string CONST_BOMTYPE = "BOM";
        private string EntryTime = "";
        private string _saleRow = "";
        private IntegrationParam param = null;
        private string _selectedDb = "Link3_Demo_DB";

        public bool IsCDN = false;
        private string ToDate;
        private string FromDate;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        #endregion

        public FormSaleImportACI()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType, bool cdn)
        {
            FormSaleImportACI form = new FormSaleImportACI();

            form.IsCDN = cdn;

            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

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

        }

        private void typeLoad()
        {
            cmbImportType.Items.Add(CONST_EXCEL);
            cmbImportType.Items.Add(CONST_TEXT);
            cmbImportType.Items.Add(CONST_DATABASE);
            // cmbImportType.Items.Add(CONST_ProcessOnly);


            CommonDAL dal = new CommonDAL();

            string value = dal.settingsDesktop("Import", "SaleImportSelection",null,connVM);
            string code = dal.settingsDesktop("CompanyCode", "Code",null,connVM);

            cmbImportType.SelectedItem = CONST_EXCEL;
            cmbImportType.Text = value;

            //dptTime.Checked = code == "CEPL";

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
                    string[] extention = fileName.Split(".".ToCharArray());
                    string[] retResults = new string[4];
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

                progressBar1.Visible = true;

                selectedType = cmbImportType.Text;
                #region Excel
                if (selectedType == CONST_EXCEL)
                {
                    ds = LoadFromExcel();


                    dtTableResult = ds.Tables["SaleM"];
                    dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] { "Delivery_Date_Time", "Invoice_Date_Time" });
                    dgvLoadedTable.DataSource = dtTableResult;
                    loadedTable = CONST_SALETYPE;
                    lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

                }
                #endregion
                #region Text
                else if (selectedType == CONST_TEXT)
                {

                    var flag = new CommonDAL().settingValue(CONST_SINGLEIMPORT, "SaleImport",connVM);

                    if (flag == "Y")
                    {
                        dtTableResult = GetTableFromText(CONST_SALETYPE);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_SALETYPE;
                    }
                    else
                    {
                        dtTableResult = GetTableFromTextDouble();
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_SALETYPE;
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

            FromDate = dtpSaleFromDate.Checked == false ? "1900-01-01" : dtpSaleFromDate.Value.ToString("yyyy-MMM-dd");
            ToDate = dtpSaleToDate.Checked == false ? "9000-12-31" : dtpSaleToDate.Value.ToString("yyyy-MMM-dd");
            //DateRange = FromDate + " - " + ToDate;

            EntryTime = dptTime.Checked ? dptTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
            
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
                    CommonDAL _cDal = new CommonDAL();
                    var importDal = new ImportDAL();

                    selectedType = cmbImportType.Text;

                    var saleDal = new SaleDAL();

                    var invoiceNo = txtId.Text.Trim();

                    //if (string.IsNullOrEmpty(invoiceNo))
                    //{
                    //    MessageBox.Show(@"Please Enter Transaction No");
                    //    return;
                    //}


                    //string value = _cDal.settingValue("Import", "SaleExistContinue");

                    //if (value == "N")
                    //{
                    //    List<SaleMasterVM> sales = saleDal.SelectAllTop1(0, new[] { "sih.ImportIDExcel" }, new[] { invoiceNo }, null, null, null, null);

                    //    if (sales != null && sales.Count > 0)
                    //    {
                    //        var sale = sales.FirstOrDefault();

                    //        if (sale != null)
                    //            MessageBox.Show(@"This Transaction No is already in system with invoice no - " +
                    //                            sale.SalesInvoiceNo);

                    //        return;
                    //    }
                    //}

                    //importDal.GetSaleDataJapha();

                    GetSearchData(invoiceNo);


                    //dtTableResult.Columns.Remove("IsVATComplete");


                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }


        }

        private void GetSearchData(string invoiceNo = "", bool withIsProcessed = false)
        {
            ImportDAL importDal = new ImportDAL();
            NullCheck();
            var dal = new BranchProfileDAL();

            DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);

            param = new IntegrationParam
            {
                TransactionType = transactionType,
                RefNo = invoiceNo,
                dtConnectionInfo = dt,
                FromDate = FromDate,
                ToDate = ToDate,
                WithIsProcessed = withIsProcessed
            };

            progressBar1.Visible = true;
            bgwSaleLoad.RunWorkerAsync();
            //dtTableResult = importDal.GetSaleACIDbData(param);
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
                    string SingleSaleImport = new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "SaleImport",null,connVM);
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
                sqlResults = saleDal.ImportData(dtSaleM, dtSaleD, dtSaleE, CommercialImporter,Program.BranchId,connVM,Program.CurrentUserID);
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
                    try
                    {
                        var result = new CommonDAL().UpdateIsVATComplete(loadedTable, null, null,connVM);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message);
                    }
                }
                MessageBox.Show(this, "Import completed successfully");
            }
            else
            {
                MessageBox.Show(this, "Import Failed");
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
                dtSaleM.Columns.Add("CommentsD");
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
                string[] masterRows = masterData.Trim().Split("\r".ToCharArray());
                string delimeter = "|";

                foreach (string mRow in masterRows)
                {
                    string[] mItems = mRow.Trim().Replace("\n", "").Split(delimeter.ToCharArray());
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
                string IssueFromBOM = new CommonDAL().settingsDesktop("IssueFromBOM", "IssueFromBOM",null,connVM);
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

        //private void SelectAll()
        //{
        //    if (chkSelectAll.Checked == true)
        //    {
        //        for (int i = 0; i < dgvLoadedTable.RowCount; i++)
        //        {
        //            dgvLoadedTable["Select", i].Value = true;
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < dgvLoadedTable.RowCount; i++)
        //        {
        //            dgvLoadedTable["Select", i].Value = false;
        //        }
        //    }
        //}

        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaleDAL salesDal = new SaleDAL();

                #region CheckExists

                //string invoiceNo = dtTableResult.Rows.Count == 0 ? "" : dtTableResult.Rows[0]["ID"].ToString();

                //if (string.IsNullOrEmpty(invoiceNo))
                //{
                //    return;
                //}

                //string value = _cDal.settingValue("Import", "SaleExistContinue");

                //if (value == "N")
                //{
                //    List<SaleMasterVM> sales = salesDal.SelectAllTop1(0, new[] { "sih.ImportIDExcel" }, new[] { invoiceNo }, null, null, null, null);

                //    if (sales != null && sales.Count > 0)
                //    {
                //        var sale = sales.FirstOrDefault();

                //        if (sale != null)
                //            MessageBox.Show(@"This Transaction No is already in system with invoice no - " +
                //                            sale.SalesInvoiceNo);

                //        return;
                //    }
                //}

                #endregion

                if (selectedType == CONST_ProcessOnly)
                {
                    sqlResults = salesDal.SaveAndProcessTempData(new DataTable(), BulkCallBack, Program.BranchId,connVM);
                }
                else
                {
                    DataTable salesData = dtTableResult.Copy();

                    salesData.Columns.Remove("InvoiceNo");


                    #region Check Multiple Save

                    CommonDAL commonDAL = new CommonDAL();

                    string allowMultiple = commonDAL.settings("Integration", "AllowMultipleSave",null,null,connVM);

                    if (allowMultiple == "N")
                    {

                        DataTable dt = salesData.DefaultView.ToTable(true, "ID");

                        if (dt.Rows.Count > 1)
                        {
                            //MessageBox.Show("Multiple Save is Not Allowed");
                            sqlResults[0] = "fail";
                            sqlResults[1] = "Multiple Save is Not Allowed";
                            return;
                        }

                    }

                    #endregion


                    IntegrationParam param = new IntegrationParam();

                    if (!string.IsNullOrEmpty(EntryTime))
                    {
                        param.InvoiceDateTime = Convert.ToDateTime(EntryTime).ToString("yyyy-MM-dd");
                        param.InvoiceTime = Convert.ToDateTime(EntryTime).ToString(" HH:mm:ss");
                        param.IsEntryDate = true;
                    }


                    TableValidation(salesData);


                    if (InvokeRequired)
                        Invoke((MethodInvoker)delegate { PercentBar(5); });

                    BulkCallBack();
                    sqlResults = salesDal.SaveAndProcess(salesData, BulkCallBack, Program.BranchId, "",connVM, param,null,null,Program.CurrentUserID);

                    UpdateOtherDB(salesData, transactionType);
                }



            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
        }

        private void UpdateOtherDB(DataTable salesData, string transactionType = "Other")
        {
            if (sqlResults[0].ToLower() == "success" && selectedType == CONST_DATABASE)
            {
                try
                {
                    ImportDAL importDal = new ImportDAL();

                    DataView view = new DataView(salesData);

                    salesData = view.ToTable(true, "ID");

                    string[] results = new string[5];

                    if (transactionType.ToLower() != "tollissue")
                    {
                        results = importDal.UpdateACISales(salesData, settingVM.BranchInfoDT,connVM);

                    }
                    else
                    {
                        results = importDal.UpdateACISales(salesData, settingVM.BranchInfoDT,connVM);

                    }
                    if (results[0].ToLower() != "success")
                    {
                        string message = "These Id failed to insert to other database\n";

                        foreach (DataRow row in salesData.Rows)
                        {
                            message += row["ID"] + "\n";
                        }

                        FileLogger.Log("ACI", "UpdateOtherDB", message);

                    }
                }
                catch (Exception e)
                {
                    string message = "These Id failed to insert to other database\n";

                    foreach (DataRow row in salesData.Rows)
                    {
                        message += row["ID"] + "\n";
                    }

                    FileLogger.Log("ACI", "UpdateOtherDB", message);

                    MessageBox.Show(e.Message + "\n" + "Staging table may not be updated");
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
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = transactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);
            salesData.Columns.Add(TransactionType);
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
                    GetSearchData("", true);
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
                        var result = new CommonDAL().UpdateIsVATComplete(loadedTable, null, null);
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
            try
            {
                #region Excel Check and Progress bar

                if (loadedTable == "")
                {
                    return;
                }

                if (!_isDeleteTemp)
                {
                    MessageBox.Show(this, "Please select new excel file");
                    return;
                }

                _selectedDb = cmbDBList.Text;
                selectedType = cmbImportType.Text;

                this.progressBar1.Visible = true;

                #endregion


                dtTableResult = dtTableResult.Copy();
                NullCheck();
                bgwBigData.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void dgvLoadedTable_DoubleClick(object sender, EventArgs e)
        {

        }

        private void dgvLoadedTable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (cmbImportType.Text == CONST_DATABASE)
            {
                _saleRow = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString();

                //var selectedRows = dgvLoadedTable.Rows[e.RowIndex];

                //var firstRow = selectedRows;

                //var invoiceNo = firstRow.Cells[0].Value.ToString();

                //var saleDal = new SaleDAL();

                //_saleRow = saleDal.GetSaleJoin(invoiceNo);

                this.Hide();
            }

        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chkOldDb_CheckedChanged(object sender, EventArgs e)
        {
            var flag = chkOldDb.Checked;

            cmbImportType.SelectedIndex = 2;
            cmbImportType.Enabled = !flag;
            cmbDBList.Visible = flag;
            lblDB.Visible = flag;

            //DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            //SqlConnection sqlConn = _dbsqlConnection.GetConnection();
            //var sqlServer = new Server(new ServerConnection(sqlConn));

            //var dbList = new List<Database>();
            //foreach (Database db in sqlServer.Databases)
            //{
            //    dbList.Add(db);
            //}


            //cmbDBList.DataSource = dbList;


            if (flag)
            {
                var saleDAL = new SaleDAL();

                var oldDbs = saleDAL.GetOldDbList(connVM);
                cmbDBList.Items.Clear();
                if (oldDbs.Rows.Count > 0)
                {
                    foreach (DataRow row in oldDbs.Rows)
                    {
                        cmbDBList.Items.Add(row["DBName"].ToString());
                    }

                    cmbDBList.SelectedIndex = 0;
                }
                else
                {
                    cmbDBList.Items.Add(Program.DatabaseName);
                    cmbDBList.Text = Program.DatabaseName;
                }
            }


        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvLoadedTable.RowCount == 0)
                    return;

                progressBar1.Visible = true;

                DialogResult dialogResult = MessageBox.Show("Are sure to post all sales?", "Post Sale", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dtTableResult = (DataTable)dgvLoadedTable.DataSource;
                    dtTableResult = dtTableResult.Copy();

                    DataView dv = new DataView(dtTableResult);

                    dtTableResult = dv.ToTable(true, "ID");
                    bgwPost.RunWorkerAsync();

                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ImportDAL importDal = new ImportDAL();

                sqlResults = importDal.PostACISaleData(dtTableResult,connVM);
            }
            catch (Exception exception)
            {
                sqlResults[0] = "fail";
                sqlResults[1] = exception.Message;
            }
        }

        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    GetSearchData("", true);
                    MessageBox.Show(this, "Posted Successfully");
                }
                else
                {
                    MessageBox.Show(this, sqlResults[1]);

                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar1.Visible = false;

            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {

                if (dgvLoadedTable.RowCount == 0)
                    return;

                DialogResult dialogResult = MessageBox.Show("Are sure to Preview all sales?", "Preview Sale", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SaleDAL saleDal = new SaleDAL();

                    #region Prepare Distinct Data

                    dtTableResult = (DataTable)dgvLoadedTable.DataSource;
                    dtTableResult = dtTableResult.Copy();

                    DataView dv = new DataView(dtTableResult);

                    dtTableResult = dv.ToTable(true, "ID","InvoiceNo");

                    #endregion

                    #region Preview Sales

                    ResultVM result = saleDal.BulkInsertMasterTemp(dtTableResult,null,null,connVM);

                    if (result.Status.ToLower() == "success")
                    {
                        string invoices = "'5'";

                        SaleReport _reportClass = new SaleReport();

                        ReportDocument reportDocument = _reportClass.Report_VAT6_3_Completed(invoices, null, transactionType.ToLower() == "credit",
                            transactionType.ToLower() == "debit", false, false, false, false
                            , true, 0, 0, false, false, false, false, true,false,connVM);


                        FormReport reports = new FormReport();
                        reports.crystalReportViewer1.Refresh();
                        reports.setReportSource(reportDocument);
                        reports.Show();
                    }
                    else
                    {
                        MessageBox.Show(result.Exception);
                    }

                    #endregion
                }


            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvLoadedTable.RowCount == 0)
                    return;
                progressBar1.Visible = true;

                DialogResult dialogResult = MessageBox.Show("Are you sure to print all sales?", "Some Title",
                    MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SaleReport _reportClass = new SaleReport();
                    CommonDAL commonDal = new CommonDAL();
                    string PrinterName = commonDal.settingsDesktop("Printer", "DefaultPrinter",null,connVM);

                    #region Prepare Distinct Data

                    dtTableResult = (DataTable) dgvLoadedTable.DataSource;
                    dtTableResult = dtTableResult.Copy();

                    DataView dv = new DataView(dtTableResult);

                    dtTableResult = dv.ToTable(true, "InvoiceNo","Post");

                    #endregion

                    #region Print All Invoices

                    foreach (DataRow row in dtTableResult.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["InvoiceNo"].ToString()) && row["Post"].ToString() == "Y")
                        {
                            ReportDocument reportDocument = _reportClass.Report_VAT6_3_Completed(
                                row["InvoiceNo"].ToString(), null, false,false, false,
                                false, false, false
                                , false, 0, 0, false, false, false, false,false,false,connVM);


                            PrinterSettings printerSettings = new PrinterSettings {PrinterName = PrinterName};
                            reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);
                        }
                        else
                        {
                            MessageBox.Show("Please Post first");
                            break;
                        }
                        
                    }

                    #endregion


                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar1.Visible = false;

            }
        }

        private void bgwSaleLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ImportDAL importDal = new ImportDAL();
                if (transactionType.ToLower() == "tollissue")
                {
                    dtTableResult = importDal.GetTollACIDbData(param,"",connVM);

                }
                else
                {
                    dtTableResult = importDal.GetSaleACIDbData(param,connVM);
                }
            }
            catch (Exception exception)
            {
                FileLogger.Log("Sale", "SaleLoad", exception.Message);
            }
        }

        private void bgwSaleLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                dgvLoadedTable.DataSource = dtTableResult;
                lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar1.Visible = false;

            }
        }

        private void txtId_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnLoad.PerformClick();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

    }
}
