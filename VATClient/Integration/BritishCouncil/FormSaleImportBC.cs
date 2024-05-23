using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
//using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using Microsoft.SqlServer.Management.Common;
using Newtonsoft.Json;
using SymphonySofttech.Reports;
using SymphonySofttech.Reports.Report;
using VATClient.ReportPreview;
using SymphonySofttech.Reports.Report.Rpt63;
using SymphonySofttech.Reports.Report.Rpt63V12V2;
using SymphonySofttech.Reports.Report.V12V2;
using VATDesktop.Repo;
using VATServer.Interface;
using Timer = System.Timers.Timer;
using System.Net.Mail;

namespace VATClient
{
    public partial class FormSaleImportBC : Form
    {
        #region Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
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
        private const string CONST_XML = "XML";
        private const string CONST_EXCEL = "Excel";
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
        private string BranchCode;
        private string BranchName;
        private string BranchLegalName;
        private string BranchAddress;
        private string BranchVatRegistrationNo;
        private DataSet ReportResultVAT11;
        private DataSet ReportResultCreditNote;
        private DataSet ReportResultDebitNote;
        private DataSet ReportResultTracking;
        private string ItemNature;
        private int AlReadyPrintNo;
        private string VAT11Name;
        private string BranchId;
        public static bool FreeSlot = true;
        #endregion

        public FormSaleImportBC()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();


        }

        public static string SelectOne(string transactionType, bool cdn)
        {
            FormSaleImportBC form = new FormSaleImportBC();

            form.IsCDN = cdn;

            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            // dgvLoadedTable.ReadOnly = true;
            tableLoad();
            typeLoad();




        }

        private void tableLoad()
        {

        }

        private void typeLoad()
        {
            cmbImportType.Items.Add(CONST_EXCEL);
            cmbImportType.Items.Add(CONST_XML);
            cmbImportType.Items.Add(CONST_DATABASE);

            // cmbImportType.Items.Add(CONST_ProcessOnly);

            CommonDAL dal = new CommonDAL();

            string value = dal.settingsDesktop("Import", "SaleImportSelection", null, connVM);

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
                #region Text and XML
                else if (selectedType == CONST_TEXT)
                {

                    var flag = new CommonDAL().settingValue(CONST_SINGLEIMPORT, "SaleImport", connVM);

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
                else if (selectedType == CONST_XML)
                {
                    dtTableResult = GetTableFromXML6();
                    dgvLoadedTable.DataSource = dtTableResult;
                    loadedTable = CONST_SALETYPE;
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
                    var importDal = new ImportDAL();
                    var dal = OrdinaryVATDesktop.GetObject<BranchProfileDAL, BranchProfileRepo, IBranchProfile>(Program.IsWCF);

                    var dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);
                    dtTableResult = importDal.GetSaleDHLAirportDbData(txtId.Text.Trim(), dt, "", connVM);
                    dgvLoadedTable.DataSource = dtTableResult;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
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
                    fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                }
                else
                {
                    //XML Files (*.xml)|*.xml|CSV files (*.csv*)|*.csv*|Text files (*.txt*)|*.txt*|
                    fdlg.Filter = "All files (*.*)|*.*";
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
                    string SingleSaleImport = new CommonDAL().settingsDesktop(CONST_SINGLEIMPORT, "SaleImport", null, connVM);
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
                    try
                    {
                        var result = new CommonDAL().UpdateIsVATComplete(loadedTable, null, null, connVM);
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
                string IssueFromBOM = new CommonDAL().settingsDesktop("IssueFromBOM", "IssueFromBOM", null, connVM);
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

        private DataTable PopulateMinimumColumn(DataTable table)
        {
            table.Rows.Clear();
            table.Columns.Clear();


            table.Columns.Add("ID");
            table.Columns.Add("Customer_Name");

            table.Columns.Add("Customer_Code");
            table.Columns.Add("Delivery_Address");
            table.Columns.Add("Invoice_Date_Time");
            table.Columns.Add("Reference_No");

            table.Columns.Add("Sale_Type");
            table.Columns.Add("Post");
            table.Columns.Add("Currency_Code");
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
            table.Columns.Add("Weight");
            table.Columns.Add("CommentsD");
            table.Columns.Add("CustomerBIN");
            table.Columns.Add("Branch_Code");
            table.Columns.Add("ReasonOfReturn");
            table.Columns.Add("DataSource");

            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "SubTotal" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "ExtraCharge" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "FileName" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "OtherRef" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "VAT_Amount" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "LineTotal" });
            table.Columns.Add(new DataColumn { DefaultValue = "Other", ColumnName = "TransactionType" });
            table.Columns.Add(new DataColumn { DefaultValue = "N", ColumnName = "IsPDFGenerated" });
            table.Columns.Add(new DataColumn { DefaultValue = "", ColumnName = "Previous_Invoice_No" });
            table.Columns.Add(new DataColumn { DefaultValue = "1900-01-01", ColumnName = "PreviousInvoiceDateTime" });
            table.Columns.Add(new DataColumn { DefaultValue = "Y", ColumnName = "Is_Print" });


            table.Columns.Add("VAT_Name");


            return table;
        }



        private DataTable GetTableFromXML()
        {
            try
            {
                string[] files = Program.ImportFileName.Split(';');

                using (StreamReader sr = new StreamReader(files[0]))
                {
                    string xml = sr.ReadToEnd();

                    DataSet invoice = OrdinaryVATDesktop.GetDataSetFromXML(xml);


                    DataTable finalTable = new DataTable();

                    finalTable = PopulateMinimumColumn(finalTable);

                    string ID = invoice.Tables["INVOICE"].Rows[0]["inv_id"].ToString();
                    string refNo = invoice.Tables["INVOICE"].Rows[0]["inv_id"].ToString();

                    DataRow addrs = invoice.Tables["CD_ADDRESSES"].Rows[0];

                    string customerName = addrs["nad_name1"].ToString();
                    string customerCode = invoice.Tables["INVOICE"].Rows[0]["inv_payer_acc"].ToString();
                    string invoiceDateTime = invoice.Tables["INVOICE"].Rows[0]["inv_insdttm"].ToString();
                    string delivaeryAdd = addrs["nad_street1"] + "\n" + addrs["nad_street2"] + "\n"
                        //  + addrs["nad_street3"] + "\n" + addrs["nad_street4"] 

                                          + "\n" + addrs["nad_postcode"] + "\n" +
                                          addrs["nad_countrycode"];


                    DataTable chargeLines = invoice.Tables["CD_CHARGELINES"];
                    DataTable weights = invoice.Tables["CD_UNIT_MEASUREMEN"];

                    weights = weights.Select("qual_value='BILLWGHT'").CopyToDataTable();

                    for (var index = 0; index < chargeLines.Rows.Count; index++)
                    {
                        DataRow row = chargeLines.Rows[index];

                        string ItemName = row["chg_descr"].ToString();
                        string price = row["chg_chgmoa_value"].ToString();
                        string priceDollar = row["chg_invmoa_value"].ToString();

                        string qunatity = row["chg_units"].ToString();

                        if (!string.IsNullOrEmpty(qunatity))
                        {
                            finalTable.Rows.Add(finalTable.NewRow());

                            int count = finalTable.Rows.Count - 1;

                            finalTable.Rows[count]["ID"] = ID;
                            finalTable.Rows[count]["Customer_Code"] = customerCode;
                            finalTable.Rows[count]["Customer_Name"] = customerName;
                            finalTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                            finalTable.Rows[count]["Reference_No"] = refNo;
                            finalTable.Rows[count]["Delivery_Address"] = delivaeryAdd;


                            finalTable.Rows[count]["Item_Name"] = ItemName;
                            finalTable.Rows[count]["Item_Code"] = invoice.Tables["CD_UNITS"].Rows[count]["unit_service"];

                            finalTable.Rows[count]["Quantity"] = qunatity;
                            decimal vatRate = 15;
                            decimal priceWithOutVat = (Convert.ToDecimal(price) * (100 / (100 + vatRate)));

                            decimal originnalPrice = priceWithOutVat / Convert.ToDecimal(qunatity);

                            finalTable.Rows[count]["NBR_Price"] =
                                originnalPrice;

                            finalTable.Rows[count]["ExtraCharge"] =
                                chargeLines.Rows[index + 1]["chg_chgmoa_value"].ToString();

                            finalTable.Rows[count]["Post"] = "Y";
                            finalTable.Rows[count]["UOM"] = "Unit";


                            finalTable.Rows[count]["Weight"] = weights.Rows[count]["mea_value"];

                            finalTable.Rows[count]["VAT_Rate"] = "15";
                            finalTable.Rows[count]["SD_Rate"] = "0";

                            finalTable.Rows[count]["Non_Stock"] = "N";
                            finalTable.Rows[count]["Trading_MarkUp"] = "0";
                            finalTable.Rows[count]["Currency_Code"] = "BDT";
                            finalTable.Rows[count]["Sale_Type"] = "NEW";
                            finalTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                            finalTable.Rows[count]["Type"] = "VAT";

                            finalTable.Rows[count]["CommentsD"] = row["chg_reference"].ToString();

                            finalTable.Rows[count]["Post"] = "Y";
                            finalTable.Rows[count]["UOM"] = "Unit";

                        }


                    }



                    return finalTable;
                }
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private DataTable GetTableFromXML6()
        {
            try
            {
                string[] files = Program.ImportFileName.Split(';');

                DataTable finalTable = new DataTable();

                string extention = Path.GetExtension(files[0]);
                BranchProfileDAL branchProfileDal = new BranchProfileDAL();

                DataTable dt = branchProfileDal.SelectAllAccountDetails(null, null, null, null, null, true, connVM);
                DataTable dtChargeCodes = branchProfileDal.SelectAllChargeCodes(null, null, null, null, null, true, connVM);

                if (extention == ".zip")
                {
                    using (FileStream file = File.Open(files[0], FileMode.Open))
                    {
                        using (ZipArchive archive = new ZipArchive(file))
                        {
                            string fileName = Path.GetFileName(files[0]);
                            fileName = fileName.Replace(".zip", "");
                            var entry = archive.GetEntry(fileName);

                            using (MemoryStream memory = new MemoryStream())
                            {
                                entry.Open().CopyTo(memory);
                                memory.Seek(0, SeekOrigin.Begin);
                                StreamReader streamReader = new StreamReader(memory);

                                string xml = streamReader.ReadToEnd();

                                string name = Path.GetFileName(files[0]);

                                SaveXmlSale(xml, name, dt, dtChargeCodes);

                                return finalTable;
                            }

                        }
                    }
                }

                using (StreamReader sr = new StreamReader(files[0]))
                {

                    string xml = sr.ReadToEnd();

                    string name = Path.GetFileName(files[0]);

                    SaveXmlSale(xml, name, dt, dtChargeCodes);

                    return finalTable;
                }

            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private DataTable GetIBS(string xml, string srcFileName, DataTable table, DataTable chargeCodes)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            DataTable finalTable = new DataTable();
            DataTable tempTable = new DataTable();

            finalTable = PopulateMinimumColumn(finalTable);
            tempTable = PopulateMinimumColumn(tempTable);


            try
            {
                XmlNodeList invoices = xmlDocument.GetElementsByTagName("INVOICE");

                ProductDAL productDal = new ProductDAL();

                foreach (XmlNode invoice in invoices)
                {
                    string ID = invoice.Attributes["Id"].InnerText;
                    string refNo = ID;
                    XmlNode customer = invoice.SelectSingleNode("ADDR[@TyCd='IV']");
                    string customerName = customer["Name"].InnerText;
                    string delivaeryAdd = "";
                    string bin = "-";
                    string saleType = "NEW";
                    string transactionType = "Other";

                    string vatType = "VAT";
                    string vatRate = "15";
                    string cnDnref = "";
                    string branch_Code = "";
                    string revenueIdentifier = "";
                    List<string> orginInvoices = new List<string>();
                    decimal grandTotal = 0;

                    string productRefNo = "";

                    #region Customer Info

                    XmlNodeList streets = customer.SelectNodes("Street");

                    foreach (XmlNode st in streets)
                    {
                        if (!string.IsNullOrEmpty(st.InnerText))
                        {
                            delivaeryAdd += st.InnerText + "\n";
                        }
                    }

                    if (customer.SelectSingleNode("PostCd") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("PostCd").InnerText))
                    {
                        delivaeryAdd += customer.SelectSingleNode("PostCd").InnerText + " ";
                    }

                    if (customer.SelectSingleNode("CtyNm") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("CtyNm").InnerText))
                    {
                        delivaeryAdd += customer.SelectSingleNode("CtyNm").InnerText;
                    }

                    if (customer.SelectSingleNode("CtryCd") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("CtryCd").InnerText))
                    {
                        RegionInfo info = new RegionInfo(customer.SelectSingleNode("CtryCd").InnerText);

                        delivaeryAdd += " " + info.DisplayName.ToUpper();
                    }

                    if (customer.SelectSingleNode("RegNum") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("RegNum").InnerText))
                    {
                        bin = customer.SelectSingleNode("RegNum").InnerText;
                    }

                    #endregion

                    string accountCode = invoice.SelectSingleNode("Acc[@TyCd='PAYER']").InnerText;
                    string invoiceDateTime = invoice.SelectSingleNode("Dtm[@TyCd='INV']").InnerText;

                    #region BranchCode

                    if (!OrdinaryVATDesktop.IsNumber(accountCode))
                    {
                        DataRow[] rows = table.Select("AccountCode = '" + accountCode + "'");

                        if (rows != null)
                        {
                            branch_Code = rows[0]["LocationCode"].ToString();
                        }
                    }
                    else
                    {
                        branch_Code = "HOF";
                    }

                    if (branch_Code.ToLower() == "hof")
                    {
                        revenueIdentifier = "A";
                    }
                    else if (branch_Code.ToLower() == "tej")
                    {
                        revenueIdentifier = "P";
                    }
                    else
                    {
                        revenueIdentifier = "R";
                    }

                    #endregion


                    //XmlNodeList test = invoice.SelectNodes("UNIT[@ProdSrvCd!='Z']");


                    XmlNode node = invoice.SelectSingleNode("//SrvA[@TyCd='ORIGIN']");

                    string billsyst = invoice.SelectSingleNode("REFS").SelectSingleNode("REF[@TyCd='BILLSYST']")
                        .InnerText;

                    #region SaleType
                    string type = invoice.SelectSingleNode("REFS").SelectSingleNode("REF[@TyCd='INVTYPE']")
                        .InnerText;
                    if (type == "N")
                    {
                        saleType = "credit";
                        transactionType = "credit";

                    }
                    else if (type == "D")
                    {
                        saleType = "debit";
                        transactionType = "debit";
                    }

                    //if (type == "N")
                    //    throw new Exception("debug");

                    #endregion

                    #region Invoice Type

                    string accCsh = "ACC";
                    if (!OrdinaryVATDesktop.IsNumber(accountCode))
                    {
                        accCsh = "CSH";
                    }

                    #endregion


                    #region File Name Generate

                    string fileName = "^" + accountCode + "^" + ID + "^" + invoiceDateTime.Replace("-", "") + "^" +
                                      node.SelectSingleNode("SrvACd").InnerText + "^^" + accCsh + "^^^^^" +
                                      invoice.SelectSingleNode("REFS").SelectSingleNode("REF[@TyCd='BILLCTRY']")
                                          .InnerText + "^^" +
                                      billsyst.Substring(0, billsyst.Length - 1) +
                                      "^^BIV^OB^EBILL^COPY_VTI_DAC_CO_SPN_@gendate_@gentime";

                    #endregion

                    invoiceDateTime += " 16:00";

                    #region line iteams
                    XmlNodeList lineItems = invoice.SelectNodes("UNIT[@ProdSrvCd!='Z']");

                    XmlNodeList lineItemsZ = invoice.SelectNodes("UNIT[@ProdSrvCd='Z']");


                    #region LineItemsRegular

                    if (lineItems != null)
                    {
                        string originInvoice = "";
                        foreach (XmlNode item in lineItems)
                        {
                            finalTable.Rows.Add(finalTable.NewRow());
                            tempTable.Rows.Add(tempTable.NewRow());

                            int count = finalTable.Rows.Count - 1;
                            originInvoice = "";

                            #region Master

                            if (string.IsNullOrEmpty(branch_Code))
                            {
                                throw new Exception("Branch Code Not Found");
                            }

                            finalTable.Rows[count]["ID"] = ID;
                            finalTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                            finalTable.Rows[count]["Customer_Name"] = customerName;
                            finalTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                            finalTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                            finalTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                            finalTable.Rows[count]["SD_Rate"] = "0";

                            finalTable.Rows[count]["Non_Stock"] = "N";
                            finalTable.Rows[count]["Trading_MarkUp"] = "0";
                            finalTable.Rows[count]["Currency_Code"] = "BDT";
                            finalTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                            finalTable.Rows[count]["Post"] = "Y";
                            finalTable.Rows[count]["DataSource"] = "IBS";
                            finalTable.Rows[count]["Branch_Code"] = branch_Code;

                            #region Temp table Data Assign

                            tempTable.Rows[count]["ID"] = ID;
                            tempTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                            tempTable.Rows[count]["Customer_Name"] = customerName;
                            tempTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                            tempTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                            tempTable.Rows[count]["Delivery_Address"] = delivaeryAdd;
                            tempTable.Rows[count]["SD_Rate"] = "0";
                            tempTable.Rows[count]["Non_Stock"] = "N";
                            tempTable.Rows[count]["Trading_MarkUp"] = "0";
                            tempTable.Rows[count]["Currency_Code"] = "BDT";
                            tempTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                            tempTable.Rows[count]["Post"] = "Y";
                            tempTable.Rows[count]["DataSource"] = "IBS";
                            tempTable.Rows[count]["Branch_Code"] = branch_Code;

                            #endregion


                            string itemCode = item.Attributes["ProdSrvCd"].InnerText;
                            string comments = item.Attributes["ItemNo"].InnerText;

                            #endregion

                            XmlNode itemDetails = item.SelectSingleNode("CHGLN[@LineNo='1']");
                            XmlNode reasonOfReturnNode = item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("FTXS");
                            XmlNode DtmDate = item.SelectSingleNode("Dtm[@TyCd='UNIT']");
                            string returnDate = DtmDate != null ? DtmDate.InnerText : "1900-01-01";
                            string reasonOfReturn = "";

                            if (reasonOfReturnNode != null)
                            {
                                var selectSingleNode = reasonOfReturnNode.SelectSingleNode("FTX[@TyCd='CLD']");
                                if (selectSingleNode != null) reasonOfReturn = selectSingleNode.InnerText;
                            }


                            string itemName = itemDetails.Attributes["Dsc"]
                                .InnerText;

                            //if (isTaxDuties != null)
                            //{
                            //    itemName = "DUTIES AND TAXES";
                            //}

                            #region UOM and Dollar Value Ref

                            List<ProductVM> products = productDal.SelectAll("0", new[] { "pr.ProductName" }, new[] { itemName }, null, null, null, connVM);

                            if (products != null && products.Any())
                            {
                                finalTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                                tempTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                            }
                            else
                            {
                                finalTable.Rows[count]["UOM"] = "Shipment";
                                tempTable.Rows[count]["UOM"] = "Shipment";
                            }


                            string otherRef = "<otherRef><dollarRate>" + itemDetails.SelectSingleNode("ExRate").InnerText +
                                              "</dollarRate>"
                                              + "<dollarValue>" + itemDetails.SelectSingleNode("Val[@TyCd='INV']")
                                                  .InnerText + "</dollarValue><fileName>" + srcFileName + "</fileName></otherRef>";

                            #endregion

                            #region Price Calculation

                            string qunatity = "1";
                            //item.SelectSingleNode("MEAS[@TyCd='EA']") == null
                            //? "1"
                            //: item.SelectSingleNode("MEAS[@TyCd='EA']").InnerText;

                            string weight = item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']") == null
                                ? "0"
                                : item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']").InnerText;


                            XmlNodeList prices = item.SelectNodes("CHGLN");

                            decimal onlyPrice = 0;
                            decimal charges = 0;

                            decimal vatAmount = 0;
                            decimal lineTotal = 0;
                            decimal TemplineTotal = 0;
                            decimal tempVatAmount = 0;

                            foreach (XmlNode price in prices)
                            {

                                XmlNode tax = price.SelectSingleNode("TAX");

                                if ((price.SelectSingleNode("TAX[@TxCd='A']") == null && price.SelectSingleNode("TAX[@TxCd='B']") == null) && tax != null) continue;


                                string priceAndChrgs = "0";

                                priceAndChrgs = tax != null
                                    ? tax.SelectSingleNode("AmtSubj[@TyCd='CHG']").InnerText
                                    : price.SelectSingleNode("Val[@TyCd='CHG']").InnerText;


                                decimal priceAndChrgs_converted = Convert.ToDecimal(priceAndChrgs);

                                if (tax == null)
                                {
                                    XmlNode DISC = price.SelectSingleNode("DISC");

                                    if (DISC != null)
                                    {
                                        priceAndChrgs_converted -=
                                            Convert.ToDecimal(DISC.SelectSingleNode("Val[@TyCd='CHG']").InnerText);
                                    }

                                }


                                if (price.Attributes["LineNo"].InnerText != "1")
                                {
                                    charges += priceAndChrgs_converted;
                                }
                                else
                                {
                                    onlyPrice += priceAndChrgs_converted;

                                    if (price.Attributes != null && price.Attributes["Units"] != null && price.Attributes["Units"].InnerText.Trim().StartsWith("-"))
                                    {
                                        productRefNo = price.Attributes["Dsc"].InnerText;
                                    }
                                }

                                if (tax != null)
                                {
                                    vatAmount += Convert.ToDecimal(tax.SelectSingleNode("Val[@TyCd='CHG']").InnerText);
                                }


                                XmlNode lineTotals = price.SelectSingleNode("TOTS");

                                XmlNode lineTotalxml = lineTotals.SelectSingleNode("TOT[@TyCd='LINETOT_LCL']");

                                lineTotal += Convert.ToDecimal(lineTotalxml.InnerText);

                            }


                            decimal
                                totalPrice =
                                    onlyPrice +
                                    charges; //item.SelectSingleNode("TOTS").SelectSingleNode("TOT[@TyCd='UNIT_TOT_LCL']").InnerText;

                            if (totalPrice == 0)
                            {
                                //throw new ArgumentException("Price cant be 0");
                                finalTable.Rows.RemoveAt(count);
                                continue;
                            }
                            decimal originnalPrice = totalPrice / Convert.ToDecimal(qunatity);
                            decimal TempOriginnalPrice = totalPrice / Convert.ToDecimal(qunatity);

                            grandTotal += (totalPrice + vatAmount);
                            tempVatAmount = vatAmount;


                            if (originnalPrice < 0)
                            {
                                originnalPrice = originnalPrice * -1;
                            }

                            if (vatAmount < 0)
                            {
                                vatAmount = vatAmount * -1;
                            }

                            TemplineTotal = lineTotal;
                            if (lineTotal < 0)
                                lineTotal *= -1;

                            #endregion


                            #region VAT Type

                            bool itemTax =
                                item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("TAX") == null ||
                                item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("TAX[@TxCd='B']") != null;

                            if (itemTax)
                            {
                                vatType = "NonVAT";
                                vatRate = "0";
                            }
                            else
                            {
                                vatType = "VAT";
                                vatRate = "15";
                            }

                            if (vatAmount == 0)
                            {
                                vatType = "NonVAT";
                                vatRate = "0";
                            }

                            #endregion


                            //decimal vatRate = 15;
                            //decimal priceWithOutVat = (Convert.ToDecimal(totalPrice) * (100 / (100 + vatRate)));

                            #region CN DN Ref

                            XmlNode lineItemRefs = item.SelectSingleNode("REFS");

                            if (lineItemRefs != null)
                            {
                                XmlNodeList orginInvoice = lineItemRefs.SelectNodes("REF[@TyCd='ORIGINVNO']");

                                if (orginInvoice != null)
                                {
                                    foreach (XmlNode xmlNode in orginInvoice)
                                    {
                                        if (!orginInvoices.Contains(xmlNode.InnerText))
                                        {
                                            orginInvoices.Add(xmlNode.InnerText);
                                        }

                                        originInvoice = xmlNode.InnerText;

                                    }
                                }

                            }

                            #endregion

                            #region Details

                            finalTable.Rows[count]["Item_Name"] = itemName;
                            finalTable.Rows[count]["Item_Code"] = "-";//itemCode;

                            finalTable.Rows[count]["Quantity"] = qunatity;

                            finalTable.Rows[count]["ExtraCharge"] = charges;
                            finalTable.Rows[count]["VAT_Amount"] = vatAmount;
                            finalTable.Rows[count]["Sale_Type"] = saleType;
                            finalTable.Rows[count]["TransactionType"] = transactionType;
                            finalTable.Rows[count]["VAT_Rate"] = vatRate;
                            finalTable.Rows[count]["Type"] = vatType;


                            finalTable.Rows[count]["LineTotal"] = lineTotal;


                            finalTable.Rows[count]["NBR_Price"] =
                                originnalPrice;

                            finalTable.Rows[count]["Weight"] =
                                weight;

                            finalTable.Rows[count]["CommentsD"] =
                                comments;

                            finalTable.Rows[count]["FileName"] =
                                fileName;

                            finalTable.Rows[count]["OtherRef"] =
                                otherRef;

                            finalTable.Rows[count]["CustomerBIN"] =
                                bin;

                            finalTable.Rows[count]["ReasonOfReturn"] =
                                reasonOfReturn;

                            finalTable.Rows[count]["Previous_Invoice_No"] =
                                originInvoice;

                            finalTable.Rows[count]["PreviousInvoiceDateTime"] =
                                returnDate;

                            #endregion

                            #region Temp Details

                            tempTable.Rows[count]["Item_Name"] = itemName;
                            tempTable.Rows[count]["Item_Code"] = "-";//itemCode;

                            tempTable.Rows[count]["Quantity"] = qunatity;

                            tempTable.Rows[count]["ExtraCharge"] = charges;
                            tempTable.Rows[count]["VAT_Amount"] = tempVatAmount;
                            tempTable.Rows[count]["Sale_Type"] = saleType;
                            tempTable.Rows[count]["TransactionType"] = transactionType;
                            tempTable.Rows[count]["VAT_Rate"] = vatRate;
                            tempTable.Rows[count]["Type"] = vatType;
                            tempTable.Rows[count]["SubTotal"] = totalPrice;

                            tempTable.Rows[count]["LineTotal"] = TemplineTotal;


                            tempTable.Rows[count]["NBR_Price"] =
                                TempOriginnalPrice;

                            tempTable.Rows[count]["Weight"] =
                                weight;

                            tempTable.Rows[count]["CommentsD"] =
                                comments;

                            tempTable.Rows[count]["FileName"] =
                                fileName;

                            tempTable.Rows[count]["OtherRef"] =
                                otherRef;

                            tempTable.Rows[count]["CustomerBIN"] =
                                bin;

                            tempTable.Rows[count]["ReasonOfReturn"] =
                                reasonOfReturn;


                            tempTable.Rows[count]["Previous_Invoice_No"] =
                                originInvoice;

                            tempTable.Rows[count]["PreviousInvoiceDateTime"] =
                                returnDate;

                            #endregion

                        }
                    }

                    #endregion

                    #region LineItemsZ

                    if (lineItemsZ != null)
                    {
                        string originInvoice = "";
                        foreach (XmlNode item in lineItemsZ)
                        {


                            XmlNode reasonOfReturnNode = item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("FTXS");
                            string reasonOfReturn = "";

                            if (reasonOfReturnNode != null)
                            {
                                reasonOfReturn = reasonOfReturnNode.SelectSingleNode("FTX[@TyCd='CLD']").InnerText;
                            }

                            XmlNode DtmDate = item.SelectSingleNode("Dtm[@TyCd='UNIT']");
                            string returnDate = DtmDate != null ? DtmDate.InnerText : "1900-01-01";

                            XmlNode itemDetails = item.SelectSingleNode("CHGLN[@LineNo='1']");


                            string weight = item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']") == null
                                ? "0"
                                : item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']").InnerText;

                            #region UOM and Dollar Value Ref

                            //List<ProductVM> products = productDal.SelectAll("0", new[] { "pr.ProductName" }, new[] { itemName });

                            //if (products != null && products.Any())
                            //{
                            //    finalTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                            //}
                            //else
                            //{
                            //    finalTable.Rows[count]["UOM"] = "Shipment";
                            //}



                            string otherRef = "<otherRef><dollarRate>" +
                                              itemDetails.SelectSingleNode("ExRate").InnerText +
                                              "</dollarRate>"
                                              + "<dollarValue>" + itemDetails.SelectSingleNode("Val[@TyCd='INV']")
                                                  .InnerText + "</dollarValue><fileName>" + srcFileName +
                                              "</fileName></otherRef>";

                            #endregion


                            XmlNodeList prices = item.SelectNodes("CHGLN");


                            foreach (XmlNode price in prices)
                            {
                                var rows = chargeCodes.Select("ChargeCode = '" +
                                                   price.Attributes["ChgCd"].InnerText.ToUpper() + "'");

                                // !price.Attributes["ChgCd"].InnerText.ToLower().StartsWith("w")
                                if (rows.Length > 0)
                                {
                                    if (price.SelectSingleNode("TAX[@TxCd='A']") == null && price.SelectSingleNode("TAX[@TxCd='B']") == null)
                                        continue;
                                }


                                finalTable.Rows.Add(finalTable.NewRow());
                                tempTable.Rows.Add(tempTable.NewRow());

                                int count = finalTable.Rows.Count - 1;

                                #region Master

                                if (string.IsNullOrEmpty(branch_Code))
                                {
                                    throw new Exception("Branch Code Not Found");
                                }

                                finalTable.Rows[count]["ID"] = ID;
                                finalTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                                finalTable.Rows[count]["Customer_Name"] = customerName;
                                finalTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                                finalTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                                finalTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                                finalTable.Rows[count]["SD_Rate"] = "0";

                                finalTable.Rows[count]["Non_Stock"] = "N";
                                finalTable.Rows[count]["Trading_MarkUp"] = "0";
                                finalTable.Rows[count]["Currency_Code"] = "BDT";
                                finalTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                                finalTable.Rows[count]["Post"] = "Y";
                                finalTable.Rows[count]["Branch_Code"] = branch_Code;

                                finalTable.Rows[count]["DataSource"] = "IBS";
                                finalTable.Rows[count]["UOM"] = "Shipment";


                                #region Temp table data assign

                                tempTable.Rows[count]["ID"] = ID;
                                tempTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                                tempTable.Rows[count]["Customer_Name"] = customerName;
                                tempTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                                tempTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                                tempTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                                tempTable.Rows[count]["SD_Rate"] = "0";

                                tempTable.Rows[count]["Non_Stock"] = "N";
                                tempTable.Rows[count]["Trading_MarkUp"] = "0";
                                tempTable.Rows[count]["Currency_Code"] = "BDT";
                                tempTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                                tempTable.Rows[count]["Post"] = "Y";
                                tempTable.Rows[count]["Branch_Code"] = branch_Code;

                                tempTable.Rows[count]["DataSource"] = "IBS";
                                tempTable.Rows[count]["UOM"] = "Shipment";

                                #endregion


                                string itemCode = item.Attributes["ProdSrvCd"].InnerText;
                                string comments = item.Attributes["ItemNo"].InnerText;

                                #endregion


                                decimal onlyPrice = 0;
                                decimal charges = 0;
                                string qunatity = "1";

                                decimal vatAmount = 0;
                                decimal tempVatAmount = 0;

                                XmlNode tax = price.SelectSingleNode("TAX");

                                string priceAndChrgs = "0";


                                string itemName = price.Attributes["Dsc"].InnerText;

                                priceAndChrgs = tax != null
                                    ? tax.SelectSingleNode("AmtSubj[@TyCd='CHG']").InnerText
                                    : price.SelectSingleNode("Val[@TyCd='CHG']").InnerText;


                                onlyPrice = Convert.ToDecimal(priceAndChrgs);

                                if (tax != null)
                                {
                                    vatAmount += Convert.ToDecimal(tax.SelectSingleNode("Val[@TyCd='CHG']").InnerText);
                                }

                                XmlNode lineTotals = price.SelectSingleNode("TOTS");

                                XmlNode lineTotalxml = lineTotals.SelectSingleNode("TOT[@TyCd='LINETOT_LCL']");

                                decimal lineTotal = Convert.ToDecimal(lineTotalxml.InnerText);
                                decimal tempLineTotal = Convert.ToDecimal(lineTotalxml.InnerText);

                                decimal
                                    totalPrice =
                                        onlyPrice +
                                        charges; //item.SelectSingleNode("TOTS").SelectSingleNode("TOT[@TyCd='UNIT_TOT_LCL']").InnerText;

                                if (totalPrice == 0)
                                {
                                    finalTable.Rows.RemoveAt(count);
                                    continue;
                                }
                                decimal originnalPrice = totalPrice / Convert.ToDecimal(qunatity);
                                decimal TemporiginnalPrice = totalPrice / Convert.ToDecimal(qunatity);

                                grandTotal += (totalPrice + vatAmount);
                                tempVatAmount = vatAmount;

                                if (originnalPrice < 0)
                                    originnalPrice = originnalPrice * -1;

                                if (vatAmount < 0)
                                    vatAmount = vatAmount * -1;

                                if (lineTotal < 0)
                                    lineTotal *= -1;


                                if (price.Attributes != null && price.Attributes["Units"] != null && price.Attributes["Units"].InnerText.Trim().StartsWith("-"))
                                {
                                    productRefNo = price.Attributes["Dsc"].InnerText;
                                }

                                #region VAT Type

                                bool itemTax = price.SelectSingleNode("TAX") == null ||
                                               price.SelectSingleNode("TAX[@TxCd='B']") != null;

                                if (itemTax)
                                {
                                    vatType = "NonVAT";
                                    vatRate = "0";
                                }
                                else
                                {
                                    vatType = "VAT";
                                    vatRate = "15";
                                }

                                if (vatAmount == 0)
                                {
                                    vatType = "NonVAT";
                                    vatRate = "0";
                                }

                                #endregion


                                #region CN DN Ref

                                XmlNode lineItemRefs = item.SelectSingleNode("REFS");

                                if (lineItemRefs != null)
                                {
                                    XmlNodeList orginInvoice = lineItemRefs.SelectNodes("REF[@TyCd='ORIGINVNO']");

                                    if (orginInvoice != null)
                                    {
                                        foreach (XmlNode xmlNode in orginInvoice)
                                        {
                                            if (!orginInvoices.Contains(xmlNode.InnerText))
                                            {
                                                orginInvoices.Add(xmlNode.InnerText);
                                            }

                                            originInvoice = xmlNode.InnerText;

                                        }
                                    }

                                }

                                #endregion

                                #region Details

                                finalTable.Rows[count]["Item_Name"] = itemName;
                                finalTable.Rows[count]["Item_Code"] = "-";//itemCode;

                                finalTable.Rows[count]["Quantity"] = qunatity;

                                finalTable.Rows[count]["ExtraCharge"] = charges;
                                finalTable.Rows[count]["VAT_Amount"] = vatAmount;
                                finalTable.Rows[count]["SubTotal"] = totalPrice;

                                finalTable.Rows[count]["Sale_Type"] = saleType;
                                finalTable.Rows[count]["TransactionType"] = transactionType;
                                finalTable.Rows[count]["VAT_Rate"] = vatRate;
                                finalTable.Rows[count]["Type"] = vatType;

                                finalTable.Rows[count]["LineTotal"] = lineTotal;


                                finalTable.Rows[count]["NBR_Price"] =
                                    originnalPrice;

                                finalTable.Rows[count]["Weight"] =
                                    weight;

                                finalTable.Rows[count]["CommentsD"] =
                                    comments;

                                finalTable.Rows[count]["FileName"] =
                                    fileName;

                                finalTable.Rows[count]["OtherRef"] =
                                    otherRef;

                                finalTable.Rows[count]["CustomerBIN"] =
                                    bin;

                                finalTable.Rows[count]["ReasonOfReturn"] =
                                    reasonOfReturn;

                                finalTable.Rows[count]["Previous_Invoice_No"] =
                                    originInvoice;
                                finalTable.Rows[count]["PreviousInvoiceDateTime"] =
                                    returnDate;

                                #endregion

                                #region Temp table

                                tempTable.Rows[count]["Item_Name"] = itemName;
                                tempTable.Rows[count]["Item_Code"] = "-";//itemCode;

                                tempTable.Rows[count]["Quantity"] = qunatity;

                                tempTable.Rows[count]["ExtraCharge"] = charges;
                                tempTable.Rows[count]["VAT_Amount"] = tempVatAmount;
                                tempTable.Rows[count]["SubTotal"] = totalPrice;

                                tempTable.Rows[count]["Sale_Type"] = saleType;
                                tempTable.Rows[count]["TransactionType"] = transactionType;
                                tempTable.Rows[count]["VAT_Rate"] = vatRate;
                                tempTable.Rows[count]["Type"] = vatType;

                                tempTable.Rows[count]["LineTotal"] = tempLineTotal;


                                tempTable.Rows[count]["NBR_Price"] =
                                    TemporiginnalPrice;

                                tempTable.Rows[count]["Weight"] =
                                    weight;

                                tempTable.Rows[count]["CommentsD"] =
                                    comments;

                                tempTable.Rows[count]["FileName"] =
                                    fileName;

                                tempTable.Rows[count]["OtherRef"] =
                                    otherRef;

                                tempTable.Rows[count]["CustomerBIN"] =
                                    bin;

                                tempTable.Rows[count]["ReasonOfReturn"] =
                                    reasonOfReturn;

                                tempTable.Rows[count]["Previous_Invoice_No"] =
                                    originInvoice;

                                tempTable.Rows[count]["PreviousInvoiceDateTime"] =
                                    returnDate;


                                #endregion


                            }


                        }
                    }

                    #endregion

                    #endregion

                    cnDnref = string.Join(",", orginInvoices);



                    #region Checking Transaction Type Logic Based on CashBD Account

                    if (accountCode.ToLower().StartsWith("cashbd"))
                    {


                        if (!string.IsNullOrEmpty(productRefNo))
                        {
                            DataRow[] rows = finalTable.Select("Item_Name= '" + productRefNo + "'");

                            if (rows.Length > 1)
                            {
                                CommonDAL commonDal = new CommonDAL();

                                tempTable = commonDal.GetCumulativeValue(tempTable, null, null, connVM);

                                tempTable.Columns.Add(new DataColumn()
                                {
                                    ColumnName = "OtherRef",
                                    DefaultValue = finalTable.Rows[0]["OtherRef"].ToString()
                                });

                                finalTable = tempTable.Copy();

                            }
                        }

                        if (grandTotal < 0)
                        {
                            saleType = "credit";
                            transactionType = "credit";

                            finalTable.Columns.Remove("TransactionType");
                            finalTable.Columns.Remove("Sale_Type");

                            finalTable.Columns.Add(new DataColumn() { ColumnName = "TransactionType", DefaultValue = transactionType });
                            finalTable.Columns.Add(new DataColumn() { ColumnName = "Sale_Type", DefaultValue = saleType });

                        }
                        else if (grandTotal > 0 && !string.IsNullOrEmpty(productRefNo))
                        {
                            saleType = "debit";
                            transactionType = "debit";

                            finalTable.Columns.Remove("TransactionType");
                            finalTable.Columns.Remove("Sale_Type");

                            finalTable.Columns.Add(new DataColumn() { ColumnName = "TransactionType", DefaultValue = transactionType });
                            finalTable.Columns.Add(new DataColumn() { ColumnName = "Sale_Type", DefaultValue = saleType });
                        }
                    }

                    #endregion


                    finalTable.Columns.Add(new DataColumn() { DefaultValue = cnDnref, ColumnName = "CNDNRef" });
                }
            }

            catch (ArgumentException e)
            {
                //FileLogger.Log("DHL form", "GetIBS", "Price can not be zero" + srcFileName);
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

            return finalTable;
        }



        private DataTable GetIAS(string xml, string srcFileName, DataTable chargeCodes)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            DataTable finalTable = new DataTable();

            finalTable = PopulateMinimumColumn(finalTable);
            ProductDAL productDal = new ProductDAL();

            XmlNodeList invoices = xmlDocument.GetElementsByTagName("INVOICE");
            string bin = "-";

            try
            {
                foreach (XmlNode invoice in invoices)
                {
                    string ID = invoice.SelectSingleNode("inv_id").InnerText; //3
                    string inv_tod = invoice.SelectSingleNode("inv_tod").InnerText;

                    XmlNode customer = invoice.SelectSingleNode("CD_ADDRESSES");
                    string customerName = customer.SelectSingleNode("nad_name1").InnerText;
                    string delivaeryAdd = "";
                    string revenueIdentifier = "D";
                    #region Customer Address

                    XmlNode street1 = customer.SelectSingleNode("nad_street1");
                    XmlNode street2 = customer.SelectSingleNode("nad_street2");
                    XmlNode street3 = customer.SelectSingleNode("nad_street3");
                    XmlNode street4 = customer.SelectSingleNode("nad_street4");
                    XmlNode nad_postcode = customer.SelectSingleNode("nad_postcode");
                    XmlNode nad_city = customer.SelectSingleNode("nad_city");
                    XmlNode nad_countrycode = customer.SelectSingleNode("nad_countrycode");
                    XmlNode nad_vat_number = customer.SelectSingleNode("nad_vat_number");

                    #endregion

                    #region Delivery Address

                    if (!string.IsNullOrEmpty(street1.InnerText))
                    {
                        delivaeryAdd += street1.InnerText;
                    }
                    if (!string.IsNullOrEmpty(street2.InnerText))
                    {
                        delivaeryAdd += "\n" + street2.InnerText;
                    }
                    if (!string.IsNullOrEmpty(street3.InnerText))
                    {
                        delivaeryAdd += "\n" + street3.InnerText;
                    }
                    if (!string.IsNullOrEmpty(street4.InnerText))
                    {
                        delivaeryAdd += "\n" + street4.InnerText;
                    }
                    if (!string.IsNullOrEmpty(nad_postcode.InnerText))
                    {
                        delivaeryAdd += "\n" + nad_postcode.InnerText;
                    }
                    if (!string.IsNullOrEmpty(nad_city.InnerText))
                    {
                        delivaeryAdd += " " + nad_city.InnerText;
                    }

                    #endregion

                    #region Finding BIN

                    if (!string.IsNullOrEmpty(nad_vat_number.InnerText))
                    {
                        bin = nad_vat_number.InnerText;
                    }

                    if (!string.IsNullOrEmpty(nad_countrycode.InnerText))
                    {
                        RegionInfo info = new RegionInfo(nad_countrycode.InnerText);
                        delivaeryAdd += " " + info.DisplayName;
                    }

                    #endregion

                    delivaeryAdd = delivaeryAdd.ToUpper();


                    string accountCode = invoice.SelectSingleNode("inv_payer_acc").InnerText; //2


                    //if (ID == "D01290155")
                    //    throw new Exception("debug");


                    string invoiceDateTime = invoice.SelectSingleNode("inv_insdttm").InnerText.ToDateString(); //4


                    XmlNodeList lineItems = invoice.SelectNodes("CD_INVOICELINE");

                    XmlNodeList totalRefs = invoice.SelectNodes("CD_TOTALS_REF");

                    XmlNodeList CD_INV_REFERENCES = invoice.SelectNodes("CD_INV_REFERENCES");

                    string BILLCTRY = "";
                    string BILLSYST = "";

                    foreach (XmlNode node in CD_INV_REFERENCES)
                    {
                        if (node["qual_value"].InnerText == "BILLCTRY")
                        {
                            BILLCTRY = node["ref_value"].InnerText;
                        }

                        if (node["qual_value"].InnerText == "BILLSYST")
                        {
                            BILLSYST = node["ref_value"].InnerText;
                        }
                    }

                    #region Find Quantity and Weight

                    decimal quantity = 0;
                    decimal wight = 0;
                    foreach (XmlNode node in totalRefs)
                    {
                        if (node["qual_value"].InnerText == "TOTEA")
                        {
                            quantity = Convert.ToDecimal(node["tot_value"].InnerText);
                        }

                        if (node["qual_value"].InnerText == "TOTCHW")
                        {
                            wight = Convert.ToDecimal(node["tot_value"].InnerText);
                        }
                    }

                    #endregion

                    string unit_origin = "";
                    string unit_dest = "";
                    string unit_date = "";
                    string refNo = "";
                    foreach (XmlNode item in lineItems)
                    {

                        XmlNode unit = item.SelectSingleNode("CD_UNITS");
                        XmlNodeList chargeLines = unit.SelectNodes("CD_CHARGELINES");


                        string itemCode = unit.SelectSingleNode("unit_service").InnerText;

                        if (string.IsNullOrEmpty(refNo))
                        {
                            refNo = unit.SelectSingleNode("unit_itemno").InnerText; // 1
                        }

                        unit_origin = unit.SelectSingleNode("unit_origin").InnerText; // 5
                        unit_dest = unit.SelectSingleNode("unit_destination").InnerText; // 5
                        unit_date = unit.SelectSingleNode("unit_date").InnerText;

                        #region comments

                        //if (chargeLines.Count == 1)
                        //{
                        //    XmlNode node = chargeLines[0];
                        //    itemName = node["chg_descr"].InnerText;
                        //    price = Convert.ToDecimal(node["chg_invmoa_value"].InnerText);
                        //}
                        //else
                        //{
                        //    foreach (XmlNode node in chargeLines)
                        //    {
                        //        if (node["chg_lineno"].InnerText == "2")
                        //        {
                        //            itemName = node["chg_descr"].InnerText;
                        //            price = Convert.ToDecimal(node["chg_invmoa_value"].InnerText);
                        //        }
                        //    }
                        //}

                        #endregion

                        foreach (XmlNode chargeLine in chargeLines)
                        {

                            decimal price = 0;

                            string itemName = "";

                            if (IsChargeCodeExists(chargeLine["chg_reference"].InnerText, chargeCodes))
                            {
                                itemName = chargeLine["chg_descr"].InnerText;
                                price = Convert.ToDecimal(chargeLine["chg_invmoa_value"].InnerText);
                            }

                            if (string.IsNullOrEmpty(itemName))
                            {
                                continue;
                            }


                            finalTable.Rows.Add(finalTable.NewRow());
                            int count = finalTable.Rows.Count - 1;

                            #region Master

                            finalTable.Rows[count]["ID"] = ID;
                            finalTable.Rows[count]["Customer_Code"] = accountCode;
                            finalTable.Rows[count]["Customer_Name"] = customerName;
                            finalTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime + " 16:00";
                            finalTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                            finalTable.Rows[count]["VAT_Rate"] = "15";
                            finalTable.Rows[count]["SD_Rate"] = "0";

                            finalTable.Rows[count]["Non_Stock"] = "N";
                            finalTable.Rows[count]["Trading_MarkUp"] = "0";
                            finalTable.Rows[count]["Currency_Code"] = "BDT";
                            finalTable.Rows[count]["Sale_Type"] = "NEW";
                            finalTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                            finalTable.Rows[count]["Type"] = "VAT";
                            finalTable.Rows[count]["Post"] = "Y";
                            finalTable.Rows[count]["Branch_Code"] = "TEJ";
                            finalTable.Rows[count]["DataSource"] = "IAS";

                            finalTable.Rows[count]["CustomerBIN"] =
                                bin;

                            #endregion

                            List<ProductVM> products = productDal.SelectAll("0", new[] { "pr.ProductName" }, new[] { itemName }, null, null, null, connVM);

                            if (products != null && products.Any())
                            {
                                finalTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                            }
                            else
                            {
                                finalTable.Rows[count]["UOM"] = "Shipment";
                            }

                            //if (string.IsNullOrEmpty(itemName))
                            //    throw new Exception("Item Name not found");



                            #region Details

                            finalTable.Rows[count]["Item_Name"] = itemName;
                            finalTable.Rows[count]["Item_Code"] = itemCode;

                            finalTable.Rows[count]["Quantity"] = 1;//quantity;
                            finalTable.Rows[count]["Weight"] = wight;


                            finalTable.Rows[count]["NBR_Price"] =
                                price;

                            finalTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;


                            finalTable.Rows[count]["OtherRef"] =
                                "<otherRef><dollarValue></dollarValue><fileName>" + srcFileName + "</fileName><type>IAS</type></otherRef>";

                            #endregion

                        }



                    }


                    if (finalTable.Rows.Count > 0)
                    {
                        string fileRefNo = finalTable.Rows[0]["Reference_No"].ToString().Split('~')[0];
                        string fileName = fileRefNo + "^" + accountCode + "^" +
                                          finalTable.Rows[0]["ID"] + "^" + invoiceDateTime.Replace("-", "") + "^" +
                                          unit_origin + "^" +
                                          unit_dest + "^" + inv_tod + "^^" + unit_date.Replace("-", "") + "^^^" + BILLCTRY +
                                          "^^" + BILLSYST + "^^BIV^IB^EBILL^COPY_VTI_DAC_CO_SPN_@gendate_@gentime";



                        finalTable.Columns.Remove("FileName");
                        finalTable.Columns.Add(new DataColumn() { ColumnName = "FileName", DefaultValue = fileName });
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return finalTable;
        }


        private bool IsChargeCodeExists(string code, DataTable codelist)
        {
            try
            {
                DataRow[] rows = codelist.Select("ChargeCode = '" + code + "'");

                return rows.Length > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string[] SaveXmlSale(string xml, string srcFileName, DataTable table, DataTable chargeCodes)
        {
            try
            {
                DataTable finalTable = new DataTable();
                bool isIBS = false;

                if (xml.Contains("<Orgntr AppCd=\"IBSPLUS\"/>") || xml.Contains("AppCd=\"IBSPLUS\""))
                {
                    finalTable = GetIBS(xml, srcFileName, table, chargeCodes);
                    isIBS = true;
                }
                else if (xml.Contains("<ref_value>IAS</ref_value>"))
                {
                    DataSet dt = OrdinaryVATDesktop.GetDataSetFromXML(xml);
                    string xmlStr = OrdinaryVATDesktop.GetXMLFromDataSet(dt);
                    finalTable = GetIAS(xmlStr, srcFileName, chargeCodes);

                    isIBS = false;
                }


                if (finalTable.Rows.Count == 0)
                {
                    throw new Exception("skip");
                }


                SaleDAL saleDal = new SaleDAL();
                TableValidation(finalTable);
                string[] res = saleDal.SaveAndProcess(finalTable, () => { }, Program.BranchId, "", connVM, null, null, null, Program.CurrentUserID);

                //string refNo = finalTable.Rows[0]["Reference_No"]
                //    .ToString()
                //    .Split('~')[0];


                //string refNo = finalTable.Rows[0]["ID"].ToString();
                ////FileLogger.Log("DHL", "Getfiles", "AfterSave" + "\n");

                //GenPDF(refNo, isIBS);

                return res;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public void GetFiles(string directoryPath)
        {
            string fileName = "first";
            //D:\VAT Import Files\Integration\DHL\1WeekData\IASExtracted
            //@"E:\VATClients\IBS"
            string filePath = @"D:\VAT Import Files\Integration\DHL\1WeekData\IASExtracted";
            filePath = directoryPath;
            CommonDAL commonDal = new CommonDAL();

            string zipRead = commonDal.settingsDesktop("Sale", "ZipRead", null, connVM);

            string currentFilePath = "";

            //FileLogger.Log("DHL", "Getfiles", "BeforeLoop");


            try
            {

                string[] files = Directory.GetFiles(filePath);

                //Directory.CreateDirectory()
                //Directory.GetParent(filePath).;

                BranchProfileDAL branchProfile = new BranchProfileDAL();

                DataTable dt = branchProfile.SelectAllAccountDetails();

                DataTable dtChargeCodes = branchProfile.SelectAllChargeCodes();

                int loopCounter = 20;

                if (files.Length < loopCounter)
                {
                    loopCounter = files.Length;
                }

                for (var index = 0; index < loopCounter; index++)
                {
                    try
                    {
                        string file = files[index];

                        if (IsFileLocked(file))
                            continue;


                        fileName = Path.GetFileName(file);
                        currentFilePath = file;

                        if (zipRead == "Y")
                        {
                            using (FileStream zipFile = File.Open(file, FileMode.Open))
                            {
                                using (ZipArchive archive = new ZipArchive(zipFile))
                                {
                                    //fileName = fileName.Replace(".zip", "");

                                    ZipArchiveEntry entry = archive.Entries.FirstOrDefault(); //archive.GetEntry(fileName);

                                    using (MemoryStream memory = new MemoryStream())
                                    {
                                        entry.Open().CopyTo(memory);
                                        memory.Seek(0, SeekOrigin.Begin);
                                        StreamReader streamReader = new StreamReader(memory);
                                        string xml = streamReader.ReadToEnd();

                                        streamReader.Close();
                                        memory.Close();
                                        zipFile.Close();

                                        string name = Path.GetFileName(file);

                                        SaveXmlSale(xml, name, dt, dtChargeCodes);

                                    }
                                }
                            }
                        }
                        else
                        {
                            using (StreamReader streamReader = new StreamReader(file))
                            {
                                string xml = streamReader.ReadToEnd();
                                streamReader.Close();
                                SaveXmlSale(xml, fileName, dt, dtChargeCodes);
                            }
                        }


                        //FileLogger.Log("DHL", "Getfiles", "AfterSave");

                        //Directory.CreateDirectory(Directory.GetParent(filePath).FullName + "\\Posted");
                        //File.Move(file, Directory.GetParent(filePath).FullName + "\\Posted\\" + fileName);
                        string destinationFile = Directory.GetParent(filePath).FullName + "\\Posted\\" + fileName;
                        //string destinationFile = AppDomain.CurrentDomain.BaseDirectory + "\\Posted\\" + fileName;

                        //FileLogger.Log("DHL", "Getfiles",
                        //"source path: " + file + "\n" + "dest path: " + destinationFile);

                        MoveFile(file, destinationFile);
                        //if (directoryPath.Contains("IAS"))
                        //{
                        //    FileLogger.Log("DHL", "Getfiles", fileName + "\n" + "AfterMove");
                        //}


                    }
                    catch (Exception e)
                    {
                        if (e.Message.ToLower() == "debug")
                        {
                            //Directory.CreateDirectory(Directory.GetParent(filePath).FullName + "\\Debug");
                            //File.Move(currentFilePath, Directory.GetParent(filePath).FullName + "\\Debug\\" + fileName);
                            MoveFile(currentFilePath, Directory.GetParent(filePath).FullName + "\\Debug\\" + fileName);

                        }
                        else if (e.Message.ToLower() == "skip")
                        {
                            MoveFile(currentFilePath, Directory.GetParent(filePath).FullName + "\\Skipped\\" + fileName);
                        }
                        else
                        {
                            //Directory.CreateDirectory(Directory.GetParent(filePath).FullName + "\\Hold");
                            //File.Move(currentFilePath, Directory.GetParent(filePath).FullName + "\\Hold\\" + fileName);

                            MoveFile(currentFilePath, Directory.GetParent(filePath).FullName + "\\Hold\\" + fileName);

                        }


                        FileLogger.Log("DHL", "Getfiles", fileName + "\n" + e.Message + "\n" + e.StackTrace);
                    }
                }

                //MessageBox.Show("Import Completed");
            }
            catch (Exception e)
            {
                //try
                //{
                //    //if (e.Message.ToLower() == "debug")
                //    //{
                //    //    Directory.CreateDirectory(Directory.GetParent(filePath).FullName + "\\Debug");
                //    //    File.Move(currentFilePath, Directory.GetParent(filePath).FullName + "\\Debug\\" + fileName);
                //    //}
                //    //else
                //    //{
                //    //    Directory.CreateDirectory(Directory.GetParent(filePath).FullName + "\\Hold");
                //    //    File.Move(currentFilePath, Directory.GetParent(filePath).FullName + "\\Hold\\" + fileName);
                //    //}


                //    //FileLogger.Log("DHL", "Getfiles", fileName + "\n" + e.Message + "\n" + e.StackTrace);
                //}
                //catch (Exception exception)
                //{
                //}


                FileLogger.Log("DHL", "Getfiles", fileName + "\n" + e.Message + "\n" + e.StackTrace);

                //GetFiles(directoryPath);
            }



        }

        private async void MoveFile(string sourceFile, string destinationFile)
        {
            try
            {
                using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
                {
                    //FileLogger.Log("DHL", "Getfiles", "source");

                    using (FileStream destinationStream = File.Create(destinationFile))
                    {
                        //FileLogger.Log("DHL", "Getfiles", "dest");

                        await sourceStream.CopyToAsync(destinationStream);
                        sourceStream.Close();
                        File.Delete(sourceFile);

                        //FileLogger.Log("DHL", "Getfiles", "delete");

                    }
                }
            }
            catch (Exception e)
            {
                FileLogger.Log("DHL", "Getfiles", e.Message + "\n" + e.StackTrace);

            }
        }


        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var salesDal = new SaleDAL();

                dtTableResult = (DataTable)dgvLoadedTable.DataSource;

                if (selectedType == CONST_ProcessOnly)
                {
                    sqlResults = salesDal.SaveAndProcessTempData(new DataTable(), BulkCallBack, Program.BranchId, connVM);
                }
                else
                {
                    var salesData = dtTableResult.Copy();

                    TableValidation(salesData);

                    if (InvokeRequired)
                        Invoke((MethodInvoker)delegate { PercentBar(5); });


                    BulkCallBack();

                    //OrdinaryVATDesktop.SaveExcel(salesData, "Test", "Test");


                    sqlResults = salesDal.SaveAndProcess(salesData, BulkCallBack, Program.BranchId, "", connVM, null, null, null, Program.CurrentUserID);
                    UpdateOtherDB(salesData);
                }



            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
        }


        private void UpdateOtherDB(DataTable salesData)
        {
            if (sqlResults[0].ToLower() == "success" && selectedType == CONST_DATABASE)
            {
                ImportDAL importDal = new ImportDAL();

                DataTable table = salesData.Copy();

                DataView view = new DataView(table);

                table = view.ToTable(true, "ID");

                string[] results = importDal.UpdateDHLTransactions(table, settingVM.BranchInfoDT, connVM);

                if (results[0].ToLower() != "success")
                {
                    string message = "These Id and SalesInvoiceNo failed to update to other database\n";

                    foreach (DataRow row in table.Rows)
                    {
                        message += row["ID"] + "-" + row["SalesInvoiceNo"] + "\n";
                    }

                    FileLogger.Log("DHL_Airport", "UpdateOtherDB", message);

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
            if (!salesData.Columns.Contains("CreatedBy"))
            {
                salesData.Columns.Add(CreatedBy);
            }
            if (!salesData.Columns.Contains("CreatedOn"))
            {
                salesData.Columns.Add(CreatedOn);
            }
            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }
            if (!salesData.Columns.Contains("TransactionType"))
            {
                salesData.Columns.Add(TransactionType);
            }
            if (!salesData.Columns.Contains("BOMId"))
            {
                salesData.Columns.Add(BOMId);
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



        private void PercentBar(int maximum)
        {
            progressBar1.Style = ProgressBarStyle.Blocks;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = maximum;

            progressBar1.Value = 0;

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
            try
            {
                if (loadedTable == "")
                {
                    return;
                }


                _selectedDb = cmbDBList.Text;
                selectedType = cmbImportType.Text;

                this.progressBar1.Visible = true;
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

            _saleRow = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString();
            this.Hide();

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

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {

            for (var i = 0; i < dgvLoadedTable.RowCount; i++)
            {
                dgvLoadedTable["Select", i].Value = chkSelectAll.Checked;
            }
        }


        private void btnSavePdf_Click(object sender, EventArgs e)
        {
            try
            {
                dgvLoadedTable.Rows[0].Cells["Reference_No"].Value.ToString();

                SaleDAL saleDal = new SaleDAL();

                List<SaleMasterVM> list = saleDal.SelectAllList(0, new[] { "sih.BranchId", "SelectTop" }, new[] { "2", "All" }, null, null, null, connVM);

                foreach (SaleMasterVM item in list)
                {
                    GenPDF(item.ImportIDExcel);
                }

                //GenPDF(dgvLoadedTable.Rows[0].Cells["Reference_No"].Value.ToString());
                //GeneratePDF();
                MessageBox.Show("PDF Generated");
            }
            catch (Exception exception)
            {

            }

        }

        private void GenPDF(string refNo, bool isIBS = true)
        {
            string invoiceNo = "";

            //FileLogger.Log("DHL", "Getfiles", "ref"+refNo + "\n");

            SaleDAL dal = new SaleDAL();

            SaleMasterVM vm = dal
                .SelectAllList(0, new[] { "sih.ImportIDExcel" }, new[] { refNo })
                .FirstOrDefault();

            invoiceNo = vm.SalesInvoiceNo;

            string fileName = vm.ImportIDExcel;
            //vm.FileName.Replace("@gendate", Convert.ToDateTime(vm.CreatedOn).ToString("yyyyMMdd"))
            //    .Replace("@gentime", Convert.ToDateTime(vm.CreatedOn).ToString("hhmmss"));

            SaleReport _report = new SaleReport();

            ReportDocument reportDocument = _report.Report_VAT6_3_Completed(invoiceNo, vm.TransactionType
                , vm.TransactionType == "credit"
                , vm.TransactionType == "debit", false
                , false
                , false
                , false
                , false, 1, 0, false, false, false
                , false, false, false, connVM);

            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "\\6.3 Files";

            if (isIBS)
            {
                fileDirectory += "\\IBS";

            }
            else
            {
                fileDirectory += "\\IAS";
            }

            Directory.CreateDirectory(fileDirectory);

            Stream st = new MemoryStream();

            st = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);

            string path = fileDirectory + "\\" + fileName + ".pdf";

            using (var output = new FileStream(path, FileMode.Create))
            {
                st.CopyTo(output);
            }

            //FileLogger.Log("DHL", "Getfiles", "After PDF" + refNo + "\n");

            st.Dispose();
            reportDocument.Close();
            reportDocument.Dispose();
        }

        private void GeneratePDF()
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();
                string FontSize = commonDal.settingsDesktop("FontSize", "VAT6_3", null, connVM);
                string Quantity6_3 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_3", null, connVM);
                string Amount6_3 = commonDal.settingsDesktop("DecimalPlace", "Amount6_3", null, connVM);
                string VATRate6_3 = commonDal.settingsDesktop("DecimalPlace", "VATRate6_3", null, connVM);
                string UnitPrice6_3 = commonDal.settingsDesktop("DecimalPlace", "UnitPrice6_3", null, connVM);

                ReportDSDAL reportDsdal = new ReportDSDAL();

                DataTable result = reportDsdal.VAT6_3("INV-BAN/0001/0820", "Y", "Y").Tables[0];
                RptVAT6_3_DHL2_V12V2 objrpt = new RptVAT6_3_DHL2_V12V2();

                #region Report Init

                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                FormulaFieldDefinitions crFormulaF = objrpt.DataDefinition.FormulaFields;


                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PGroupInReport", "");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TrackingTrace", "");



                objrpt.DataDefinition.FormulaFields["Quantity"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["SDAmount"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["Qty_UnitCost"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["Qty_UnitCost_SDAmount"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["VATAmount"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["Subtotal"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + ItemNature + "'";



                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_3", Quantity6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_3", Amount6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchId", BranchId);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchCode", BranchCode);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchLegalName", BranchLegalName);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchAddress", BranchAddress);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "RegistredAddress", "");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address", "");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchVatRegistrationNo", BranchVatRegistrationNo);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATRate6_3", VATRate6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "DP_UnitPrice_6_3", UnitPrice6_3);

                objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";


                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PrintDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                #endregion

                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                string fileDirectory = pathRoot + "\\6.3 Files";

                fileDirectory += "\\IBS";

                Directory.CreateDirectory(fileDirectory);

                string serialNo = result.Rows[0]["SerialNo"].ToString();

                if (serialNo.Contains("~"))
                {
                    string[] splitData = serialNo.Split('~');

                    if (splitData.Length == 3)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "AccountCode", splitData[1]);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "RevenueType", splitData[2]);
                    }
                }
                objrpt.SetDataSource(result);

                Stream st = new MemoryStream();
                st = objrpt.ExportToStream(ExportFormatType.PortableDocFormat);

                string path = fileDirectory + "\\" + "test1" + ".pdf";

                using (var output = new FileStream(path, FileMode.Create))
                {
                    st.CopyTo(output);
                }

                st.Dispose();

                #region Comments

                //result = reportDsdal.VAT11ReportNew("INV-BAN/0002/0820", "Y", "Y").Tables[0];


                //objrpt.SetDataSource(result);


                //st = objrpt.ExportToStream(ExportFormatType.PortableDocFormat);

                //path = fileDirectory + "\\" + "test2" + ".pdf";

                //using (var output = new FileStream(path, FileMode.Create))
                //{
                //    st.CopyTo(output);
                //}

                //st.Dispose();

                #endregion

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool isProcessRunning;

        string ibsPath = @"D:\IBS_IAS_Invoice\IBS\Work";
        string iasPath = @"D:\IBS_IAS_Invoice\IAS\Work";

        string ibsPath1 = @"D:\VAT Import Files\Integration\DHL\31 Dec 2020\IBS_IAS_Invoice\IBS\work";
        string iasPath1 = @"D:\VAT Import Files\Integration\DHL\31 Dec 2020\IBS_IAS_Invoice\IBS\Posted";

        //D:\IBS_IAS_Invoice\IBS\Work
        //D:\IBS_IAS_Invoice\IAS\Work
        //

        /*@"D:\VAT Import Files\Integration\DHL\1WeekData\IBS";
        @"D:\VAT Import Files\Integration\DHL\1WeekData\IASExtracted"; */

        private FileSystemWatcher ibsWatcher = null;
        private FileSystemWatcher iasWatcher = null;

        private void btnBulk_Click(object sender, EventArgs e)
        {
            //Timer timer = new Timer(15000);
            //timer.Elapsed += GetFiles(ibsPath);
            //timer.Enabled = true;
            try
            {
                #region Old Process Comment

                //FileLogger.Log("Start default IBS Process", "CheckfilesIBS", DateTime.Now.ToString());
                //GetFiles(ibsPath);
                //FileLogger.Log("End default IBS Process", "CheckfilesIBS", DateTime.Now.ToString());


                //FileLogger.Log("Start default IAS Process", "CheckfilesIAS", DateTime.Now.ToString());
                //GetFiles(iasPath);
                //FileLogger.Log("End default IAS Process", "CheckfilesIAS", DateTime.Now.ToString());

                //ibsWatcher = new FileSystemWatcher(ibsPath);
                //ibsWatcher.Created += CheckfilesIBS;
                //ibsWatcher.Filter = "*.zip";
                //ibsWatcher.EnableRaisingEvents = true;


                //iasWatcher = new FileSystemWatcher(iasPath);
                //iasWatcher.Created += CheckfilesIAS;
                //iasWatcher.Filter = "*.zip";
                //iasWatcher.NotifyFilter = NotifyFilters.FileName;
                //iasWatcher.EnableRaisingEvents = true;

                #endregion

                #region Comments

                //if (chkTest.Checked)
                //{
                //    ibsPath = @"D:\IBS_IAS_Invoice_Test\IBS_Files\Work";
                //    iasPath = @"D:\IBS_IAS_Invoice_Test\IAS_Files\Work";
                //}
                //else
                //{
                //    ibsPath = @"D:\IBS_IAS_Invoice\IBS\Work";
                //    iasPath = @"D:\IBS_IAS_Invoice\IAS\Work";
                //}

                //Directory.CreateDirectory(Directory.GetParent(ibsPath).FullName + "\\Hold");
                //Directory.CreateDirectory(Directory.GetParent(ibsPath).FullName + "\\Debug");
                //Directory.CreateDirectory(Directory.GetParent(ibsPath).FullName + "\\Posted");
                //Directory.CreateDirectory(Directory.GetParent(ibsPath).FullName + "\\Skipped");

                //Directory.CreateDirectory(Directory.GetParent(iasPath).FullName + "\\Hold");
                //Directory.CreateDirectory(Directory.GetParent(iasPath).FullName + "\\Debug");
                //Directory.CreateDirectory(Directory.GetParent(iasPath).FullName + "\\Posted");
                //Directory.CreateDirectory(Directory.GetParent(iasPath).FullName + "\\Skipped");

                #endregion

                if (!timer1.Enabled)
                {
                    timer1.Start();
                }

                MessageBox.Show("Process Started");

            }
            catch (Exception exception)
            {
                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = "0";
                    evm.FileName = "PDFGenerate";
                    evm.ErrorMassage = exception.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "btnBulk_Click";
                    evm.ActionName = "FormSaleImportBC";
                    evm.TransactionName = "Sales";

                    CommonDAL _cDal = new CommonDAL();

                    string[] Logresult = _cDal.InsertErrorLogs(evm);

                }
                catch (Exception ex)
                {

                }

                FileLogger.Log("Start FIle Watcher", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);
            }

        }

        private void CheckfilesIBS(Object source, FileSystemEventArgs e)
        {
            FormSaleImportBC form = new FormSaleImportBC();
            form.transactionType = "Other";
            try
            {
                Invoke(new MethodInvoker(() => { progressBar1.Visible = true; }));

                if (!isProcessRunning)
                {
                    isProcessRunning = true;

                    Thread.Sleep(60000);
                    FileLogger.Log("Start IBS Process", "CheckfilesIBS", DateTime.Now.ToString());

                    GetFiles(ibsPath);
                    FileLogger.Log("End IBS Process", "CheckfilesIBS", DateTime.Now.ToString());


                    Thread.Sleep(60000);
                    GetFiles(iasPath);

                    isProcessRunning = false;

                }
                Invoke(new MethodInvoker(() => { progressBar1.Visible = false; }));


            }
            catch (Exception exception)
            {
                FileLogger.Log("checkChangesIBS", "CheckfilesIBS", exception.Message + "\n" + exception.StackTrace);

            }
        }

        private void CheckfilesIAS(Object source, FileSystemEventArgs e)
        {
            FormSaleImportBC form = new FormSaleImportBC();
            form.transactionType = "Other";


            try
            {
                Invoke(new MethodInvoker(() => { progressBar1.Visible = true; }));

                if (!isProcessRunning)
                {
                    isProcessRunning = true;
                    Thread.Sleep(60000);
                    FileLogger.Log("Start IAS Process", "CheckfilesIAS", DateTime.Now.ToString());

                    GetFiles(iasPath);
                    FileLogger.Log("End IAS Process", "CheckfilesIAS", DateTime.Now.ToString());


                    Thread.Sleep(60000);

                    GetFiles(ibsPath);
                    isProcessRunning = false;


                }
                Invoke(new MethodInvoker(() => { progressBar1.Visible = false; }));




            }
            catch (Exception exception)
            {
                FileLogger.Log("checkChangesIBS", "CheckfilesIAS", exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void btnDependency_Click(object sender, EventArgs e)
        {

            string loginInfo = JsonConvert.SerializeObject(new
            {
                Program.BranchCode,
                Program.BranchId,
                Program.CurrentUser,
                Program.CurrentUserID,
                SysDBInfoVM.SysdataSource,
                DatabaseInfoVM.DatabaseName,
                SysDBInfoVM.SysPassword,
                SysDBInfoVM.SysUserName,
                SysDBInfoVM.IsWindowsAuthentication,
                Program.CompanyID
            });

            SubmitToProcessUsingClient(loginInfo);
        }

        private void SubmitToProcessUsingClient(string message)
        {
            NamedPipeClientStream pipeClient =
                new NamedPipeClientStream(".", @"VATPipe",
                    PipeDirection.InOut, PipeOptions.None,
                    TokenImpersonationLevel.Impersonation);

            pipeClient.Connect();

            StreamReadWriteToString ss = new StreamReadWriteToString(pipeClient);
            // Validate the server's signature string 
            // if the server's signature does not matched then pipe goes to broken state.

            if (ss.ReadString() == "VAT123")
            {
                // if the signatured matched with server, then write the file name again to the pipe. 
                //Then server will start to process the request
                ss.WriteString(message);
                var serverMessage = ss.ReadString();

                // process serverMessage in client side
            }


            pipeClient.Close();

        }

        private void stopBulk_Click(object sender, EventArgs e)
        {
            try
            {
                //if (iasWatcher != null)
                //{
                //    iasWatcher.EnableRaisingEvents = false;
                //}

                //if (ibsWatcher != null)
                //{
                //    ibsWatcher.EnableRaisingEvents = false;
                //}

                if (timer1.Enabled)
                {
                    timer1.Stop();
                }

                MessageBox.Show("Process Stopped");

            }
            catch (Exception exception)
            {
                FileLogger.Log("stopBulk_Click", "stopBulk_Click", exception.Message + "\n" + exception.StackTrace);
                MessageBox.Show(exception.Message);
            }
        }

        private void bgwDHL_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetFiles(ibsPath);


                //FileLogger.Log("Start default IAS Process", "CheckfilesIAS", DateTime.Now.ToString());
                //GetFiles(iasPath);
                //FileLogger.Log("End default IAS Process", "CheckfilesIAS", DateTime.Now.ToString());

                //ibsWatcher = new FileSystemWatcher(ibsPath);
                //ibsWatcher.Created += CheckfilesIBS;
                //ibsWatcher.Filter = "*.zip";
                //ibsWatcher.EnableRaisingEvents = true;


                //iasWatcher = new FileSystemWatcher(iasPath);
                //iasWatcher.Created += CheckfilesIAS;
                //iasWatcher.Filter = "*.zip";
                //iasWatcher.NotifyFilter = NotifyFilters.FileName;
                //iasWatcher.EnableRaisingEvents = true;


            }
            catch (Exception exception)
            {
                FileLogger.Log("Start FIle Watcher", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);

            }
        }

        private void bgwDHL_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FileLogger.Log("End default IBS Process", "CheckfilesIBS", DateTime.Now.ToString());
            isProcessRunning = false;
            progressBar1.Visible = false;

            //RunIAS();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!isProcessRunning)
                {

                    progressBar1.Visible = true;
                    isProcessRunning = true;
                    bgwReportGeneration.RunWorkerAsync();

                }

            }
            catch (Exception exception)
            {
                FileLogger.Log("timer1_Tick", "timer1_Tick", exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void RunIAS()
        {
            FileLogger.Log("Start default IAS Process", "CheckfilesIAS", DateTime.Now.ToString());

            //MessageBox.Show("IAS Started");

            //MessageBox.Show("IAS Started");

        }

        private void bgwIAS_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetFiles(iasPath);

            }
            catch (Exception exception)
            {
                FileLogger.Log("Start FIle Watcher", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);

            }

        }

        private void bgwIAS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FileLogger.Log("End default IAS Process", "CheckfilesIAS", DateTime.Now.ToString());
            isProcessRunning = false;
            progressBar1.Visible = false;
        }

        private void chkTest_CheckedChanged(object sender, EventArgs e)
        {

        }

        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;
        private bool IsFileLocked(string file)
        {
            //check that problem is not in destination file
            if (File.Exists(file) == true)
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex2)
                {
                    //_log.WriteLog(ex2, "Error in checking whether file is locked " + file);
                    int errorCode = Marshal.GetHRForException(ex2) & ((1 << 16) - 1);
                    if ((ex2 is IOException) && (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
                    {
                        return true;
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return false;
        }

        private void bgwReportGeneration_DoWork(object sender, DoWorkEventArgs e)
        {
            RptVAT6_3_ACI_V12V2 objrpt = null;
            ReportDocument RptVAT6_7_ACI_New_V12V2 = null;
            ReportDocument RptVAT6_8_ACI_New_V12V2 = null;
            ImportDAL dal = new ImportDAL();

            DataTable dtSales = new DataTable();

            try
            {
                DataTable result = new DataTable();
                DataTable branch = new DataTable();
                CommonDAL commonDal = new CommonDAL();
                DataTable tempResult = new DataTable();
                DataTable tempBranch = new DataTable();

                #region Credit Note & Debit Note


                #region Read Settings

                string VehicleRequired = commonDal.settingsDesktop("Sale", "VehicleRequired", null, connVM);
                string InEnglish = commonDal.settingsDesktop("Reports", "VAT6_3English", null, connVM);
                string VAT6_7English = commonDal.settingsDesktop("Reports", "VAT6_7English", null, connVM);
                string VAT6_8English = commonDal.settingsDesktop("Reports", "VAT6_8English", null, connVM);
                string companyCode = commonDal.settingsDesktop("CompanyCode", "Code", null, connVM);
                string FontSize6_7 = commonDal.settingsDesktop("FontSize", "VAT6_7", null, connVM);

                #endregion


                RptVAT6_7_ACI_New_V12V2 = new RptVAT6_7_ACI_New_V12V2();

                RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields["CompanyName"].Text =
                    "'" + OrdinaryVATDesktop.CompanyName + "'";
                RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields["TelephoneNo"].Text =
                    "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields["VatRegistrationNo"].Text =
                    "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                //FormulaFieldDefinitions crFormulaF;
                FormulaFieldDefinitions crFormulaFCredit = RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethodCredit = new CommonFormMethod();
                _vCommonFormMethodCredit.FormulaField(RptVAT6_7_ACI_New_V12V2, crFormulaFCredit, "PrintDate",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                _vCommonFormMethodCredit.FormulaField(RptVAT6_7_ACI_New_V12V2, crFormulaFCredit, "InEnglish", VAT6_7English);
                _vCommonFormMethodCredit.FormulaField(RptVAT6_7_ACI_New_V12V2, crFormulaFCredit, "FontSize", FontSize6_7);

                RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                RptVAT6_7_ACI_New_V12V2.DataDefinition.FormulaFields["Preview"].Text = "''";

                RptVAT6_8_ACI_New_V12V2 = new RptVAT6_8_ACI_New_V12V2();

                RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields["CompanyName"].Text =
                    "'" + OrdinaryVATDesktop.CompanyName + "'";
                RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields["TelephoneNo"].Text =
                    "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields["VatRegistrationNo"].Text =
                    "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                //FormulaFieldDefinitions crFormulaF;
                FormulaFieldDefinitions crFormulaDebit = RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethodDebit = new CommonFormMethod();
                _vCommonFormMethodDebit.FormulaField(RptVAT6_8_ACI_New_V12V2, crFormulaDebit, "PrintDate",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                _vCommonFormMethodDebit.FormulaField(RptVAT6_8_ACI_New_V12V2, crFormulaDebit, "InEnglish", VAT6_7English);
                _vCommonFormMethodDebit.FormulaField(RptVAT6_8_ACI_New_V12V2, crFormulaDebit, "FontSize", FontSize6_7);

                RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                RptVAT6_8_ACI_New_V12V2.DataDefinition.FormulaFields["Preview"].Text = "''";

                #endregion

                #region 6.3 Sale

                string FontSize = commonDal.settingsDesktop("FontSize", "VAT6_3", null, connVM);
                string Quantity6_3 = commonDal.settingsDesktop("DecimalPlace", "Quantity6_3", null, connVM);
                string Amount6_3 = commonDal.settingsDesktop("DecimalPlace", "Amount6_3", null, connVM);
                string VATRate6_3 = commonDal.settingsDesktop("DecimalPlace", "VATRate6_3", null, connVM);
                string UnitPrice6_3 = commonDal.settingsDesktop("DecimalPlace", "UnitPrice6_3", null, connVM);

                ReportDSDAL reportDsdal = new ReportDSDAL();

                objrpt = new RptVAT6_3_ACI_V12V2();

                #region Report Init

                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                FormulaFieldDefinitions crFormulaF = objrpt.DataDefinition.FormulaFields;

                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PGroupInReport", "");
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TrackingTrace", "");

                objrpt.DataDefinition.FormulaFields["Quantity"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["SDAmount"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["Qty_UnitCost"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["Qty_UnitCost_SDAmount"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["VATAmount"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["Subtotal"].Text = "'" + "0" + "'";
                objrpt.DataDefinition.FormulaFields["ItemNature"].Text = "'" + ItemNature + "'";

                crFormulaF = objrpt.DataDefinition.FormulaFields;
                _vCommonFormMethod = new CommonFormMethod();


                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", FontSize);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Quantity6_3", Quantity6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Amount6_3", Amount6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "VATRate6_3", VATRate6_3);
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "DP_UnitPrice_6_3", UnitPrice6_3);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + EntryUserName + "'";
                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'VAT 11'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + OrdinaryVATDesktop.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + OrdinaryVATDesktop.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + OrdinaryVATDesktop.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + OrdinaryVATDesktop.Address3 + "'";
                objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + OrdinaryVATDesktop.TelephoneNo + "'";
                objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + OrdinaryVATDesktop.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text =
                    "'" + OrdinaryVATDesktop.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["QtyInWord"].Text = "'" + QtyInWord + "'";

                objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + OrdinaryVATDesktop.Trial + "'";
                objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";

                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PrintDate",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


                #endregion


                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                string fileDirectory = pathRoot + "\\6.3 Files";

                fileDirectory += "\\TempPDF";


                Directory.CreateDirectory(fileDirectory);


                #endregion

                // get loop counter
                BranchProfileDAL branchProfile = new BranchProfileDAL();

                branch = branchProfile.SelectAll();

                int counter = dal.GetLoopCounter();

                if (counter > 0)
                {
                    string source = "-";
                    string PdfFlag = "N";

                    //RptVAT6_3_ACI_V12V2 objrpt = null;
                    //ReportDocument RptVAT6_7_ACI_New_V12V2 = null;
                    //ReportDocument RptVAT6_8_ACI_New_V12V2 = null;

                    dtSales = PdfGeneration(fileDirectory, counter, reportDsdal, source, PdfFlag, dtSales, branch,
                        _vCommonFormMethod, objrpt, crFormulaF, RptVAT6_7_ACI_New_V12V2, RptVAT6_8_ACI_New_V12V2, pathRoot, dal);
                }

                else
                {
                    // handle errors

                    int counterError = dal.GetLoopCounter("E");
                    if (counterError > 0)
                    {
                        string source = "-";
                        string PdfFlag = "E";
                        dtSales = PdfGeneration(fileDirectory, counterError, reportDsdal, source, PdfFlag, dtSales, branch,
                            _vCommonFormMethod, objrpt, crFormulaF, RptVAT6_7_ACI_New_V12V2, RptVAT6_8_ACI_New_V12V2, pathRoot, dal);
                    }

                }

            }
            catch (Exception exception)
            {
                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = "0";
                    evm.FileName = "PDFGenerate";
                    evm.ErrorMassage = exception.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "bgwReportGeneration_DoWork";
                    evm.ActionName = "FormSaleImportBC";
                    evm.TransactionName = "Sales";

                    CommonDAL _cDal = new CommonDAL();

                    string[] result = _cDal.InsertErrorLogs(evm);

                }
                catch (Exception ex)
                {

                }
                //dal.updateDHLPdfFlag(dtSales, "E");

            }
            finally
            {
                if (objrpt != null)
                {
                    objrpt.Close();
                    objrpt.Dispose();
                }

                if (RptVAT6_7_ACI_New_V12V2 != null)
                {
                    RptVAT6_7_ACI_New_V12V2.Close();
                    RptVAT6_7_ACI_New_V12V2.Dispose();
                }

                if (RptVAT6_8_ACI_New_V12V2 != null)
                {
                    RptVAT6_8_ACI_New_V12V2.Close();
                    RptVAT6_8_ACI_New_V12V2.Dispose();
                }
            }
        }

        private DataTable IASPDFGeneration(string fileDirectory, int iasCounter, ReportDSDAL reportDsdal, string source,
            string PdfFlag, DataTable dtSales, DataTable branch, CommonFormMethod _vCommonFormMethod,
            RptVAT6_3_DHL2_V12V2 objrpt, FormulaFieldDefinitions crFormulaF, string pathRoot, ImportDAL dal)
        {
            DataTable result;
            DataTable tempBranch;
            DataTable tempResult;
            // delete from temp table

            for (int i = 0; i < iasCounter; i++)
            {
                DeleteFiles(fileDirectory);

                result = reportDsdal.VAT6_3("", "Y", "Y", "n", null, false, source, PdfFlag).Tables[0];

                dtSales = result.DefaultView.ToTable(true, "SalesInvoiceNo", "BranchId",
                    "TransactionType", "DataSource");

                // group data


                foreach (DataRow dtSalesRow in dtSales.Rows)
                {
                    tempBranch = branch.Select("BranchId='" + dtSalesRow["BranchId"] + "'").CopyToDataTable();
                    tempResult = result.Select("SalesInvoiceNo = '" + dtSalesRow["SalesInvoiceNo"] + "'")
                        .CopyToDataTable();

                    if (dtSalesRow["TransactionType"].ToString().ToLower() == "other")
                    {
                        #region Data Group By

                        tempResult = GroupByResult(tempResult);

                        #endregion

                        #region Switch

                        BranchId = tempBranch.Rows[0]["BranchCode"].ToString();
                        BranchCode = tempBranch.Rows[0]["BranchCode"].ToString();
                        BranchName = tempBranch.Rows[0]["BranchName"].ToString();
                        BranchLegalName = tempBranch.Rows[0]["BranchLegalName"].ToString();
                        BranchAddress = tempBranch.Rows[0]["Address"].ToString();
                        BranchVatRegistrationNo = tempBranch.Rows[0]["VatRegistrationNo"].ToString();


                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchId", BranchId);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchCode", BranchCode);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchLegalName", BranchLegalName);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchAddress", BranchAddress);
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "RegistredAddress", "");
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address", "");
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchVatRegistrationNo",
                            BranchVatRegistrationNo);

                        string serialNo = tempResult.Rows[0]["SerialNo"].ToString();

                        if (serialNo.Contains("~"))
                        {
                            string[] splitData = serialNo.Split('~');

                            if (splitData.Length == 3)
                            {
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "AccountCode",
                                    splitData[1]);
                                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "RevenueType",
                                    splitData[2]);
                            }
                        }

                        #endregion


                        objrpt.SetDataSource(tempResult);

                        string fileName = tempResult.Rows[0]["FileName"].ToString();

                        fileName = fileName.Replace("@gendate", DateTime.Now.ToString("yyyyMMdd"))
                            .Replace("@gentime", DateTime.Now.ToString("hhmmss"));

                        ExportPDf(objrpt, fileDirectory, fileName);
                    }
                }

                // movie to IBS folder
                //MoveToFolder(fileDirectory, pathRoot + "\\6.3 Files\\" + "IAS");
                MoveToFolder(fileDirectory, "D:\\invoice");

                //MoveToFolder(pathRoot + "\\6.3 Files\\" + "IAS", "D:\\invoice");


                // update pdf flag
                dal.updateDHLPdfFlag(dtSales, "Y", connVM);
            }

            return dtSales;
        }

        private DataTable PdfGeneration(string fileDirectory, int ibsCounter, ReportDSDAL reportDsdal, string source,
            string PdfFlag, DataTable dtSales, DataTable branch, CommonFormMethod _vCommonFormMethod,
            RptVAT6_3_ACI_V12V2 objrpt, FormulaFieldDefinitions crFormulaF, ReportDocument RptVAT6_7_ACI_New_V12V2,
            ReportDocument RptVAT6_8_ACI_New_V12V2, string pathRoot, ImportDAL dal)
        {
            DataTable result;
            DataTable tempBranch;
            DataTable tempResult;
            // delete from temp table

            for (int i = 0; i < ibsCounter; i++)
            {
                try
                {
                    DeleteFiles(fileDirectory);

                    result = reportDsdal.VAT6_3("", "Y", "Y", "n", null, false, source, PdfFlag).Tables[0];

                    dtSales = result.DefaultView.ToTable(true, "SalesInvoiceNo", "BranchId", "TransactionType", "DataSource");

                    // group data

                    foreach (DataRow dtSalesRow in dtSales.Rows)
                    {
                        tempBranch = branch.Select("BranchId='" + dtSalesRow["BranchId"] + "'").CopyToDataTable();
                        var invocieNo = dtSalesRow["SalesInvoiceNo"].ToString();
                        tempResult = result.Select("SalesInvoiceNo = '" + invocieNo + "'")
                            .CopyToDataTable();

                        invocieNo = invocieNo.Replace("\\", "-").Replace("/", "-");

                        string tType = dtSalesRow["TransactionType"].ToString();

                        if (tType.ToLower() == "other" || tType.ToLower() == "exportservicens" || tType.ToLower() == "servicens")
                        {
                            #region Data Group By

                            //tempResult = GroupByResult(tempResult);

                            #endregion

                            #region Switch

                            BranchId = tempBranch.Rows[0]["BranchCode"].ToString();
                            BranchCode = tempBranch.Rows[0]["BranchCode"].ToString();
                            BranchName = tempBranch.Rows[0]["BranchName"].ToString();
                            BranchLegalName = tempBranch.Rows[0]["BranchLegalName"].ToString();
                            BranchAddress = tempBranch.Rows[0]["Address"].ToString();
                            BranchVatRegistrationNo = tempBranch.Rows[0]["VatRegistrationNo"].ToString();

                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchId", BranchId);
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchCode", BranchCode);
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchLegalName", BranchLegalName);
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchAddress", BranchAddress);
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "RegistredAddress", "");
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address", "");
                            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchVatRegistrationNo",
                                BranchVatRegistrationNo);

                            #endregion

                            objrpt.SetDataSource(tempResult);

                            string fileName = invocieNo + "~" + DateTime.Now.ToString("yyyyMMdd") + "~" + DateTime.Now.ToString("hhmmss");

                            //fileName = fileName.Replace("@gendate", DateTime.Now.ToString("yyyyMMdd"))
                            //    .Replace("@gentime", DateTime.Now.ToString("hhmmss"));

                            ExportPDf(objrpt, fileDirectory, fileName);

                            EmailProcess(invocieNo, fileDirectory, fileName);

                        }
                        else if (tType.ToLower() == "credit")
                        {
                            // group by 
                            //tempResult = CNDNGroupBy(tempResult);

                            RptVAT6_7_ACI_New_V12V2.SetDataSource(tempResult);

                            string fileName = invocieNo + "~" + DateTime.Now.ToString("yyyyMMdd") + "~" + DateTime.Now.ToString("hhmmss");

                            //fileName = fileName.Replace("@gendate", DateTime.Now.ToString("yyyyMMdd"))
                            //    .Replace("@gentime", DateTime.Now.ToString("hhmmss"));

                            ExportPDf(RptVAT6_7_ACI_New_V12V2, fileDirectory, fileName);
                        }
                        else if (tType.ToLower() == "debit")
                        {
                            // group by 

                            //tempResult = CNDNGroupBy(tempResult);

                            RptVAT6_8_ACI_New_V12V2.SetDataSource(tempResult);

                            string fileName = invocieNo + "~" + DateTime.Now.ToString("yyyyMMdd") + "~" + DateTime.Now.ToString("hhmmss");

                            //fileName = fileName.Replace("@gendate", DateTime.Now.ToString("yyyyMMdd"))
                            //    .Replace("@gentime", DateTime.Now.ToString("hhmmss"));

                            ExportPDf(RptVAT6_8_ACI_New_V12V2, fileDirectory, fileName);
                        }
                    }

                    //string a = "CRN-GTW-0002-0822";

                    MoveToFolder(fileDirectory, "D:\\invoice");

                    // update pdf flag
                    dal.updateDHLPdfFlag(dtSales, "Y", connVM);

                }
                catch (Exception e)
                {
                    try
                    {
                        ErrorLogVM evm = new ErrorLogVM();

                        evm.ImportId = "0";
                        evm.FileName = "PDFGenerate";
                        evm.ErrorMassage = e.Message;
                        evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        evm.SourceName = "PdfGeneration";
                        evm.ActionName = "FormSaleImportBC";
                        evm.TransactionName = "Sales";

                        CommonDAL _cDal = new CommonDAL();

                        string[] Logresult = _cDal.InsertErrorLogs(evm);

                    }
                    catch (Exception ex)
                    {

                    }

                    dal.updateDHLPdfFlag(dtSales, "E");

                }
            }

            return dtSales;
        }

        private DataTable CNDNGroupBy(DataTable tempResult)
        {
            var newSort = (from row in tempResult.AsEnumerable()
                           group row by new
                           {
                               SalesInvoiceNo = row.Field<string>("SalesInvoiceNo"),
                               InvoiceDate = row.Field<string>("InvoiceDate"),
                               CustomerName = row.Field<string>("CustomerName"),
                               ProductName = row.Field<string>("ProductName"),
                               Address1 = row.Field<string>("Address1"),
                               Address2 = row.Field<string>("Address2"),
                               Address3 = row.Field<string>("Address3"),
                               TelephoneNo = row.Field<string>("TelephoneNo"),
                               DeliveryAddress1 = row.Field<string>("DeliveryAddress1"),
                               DeliveryAddress2 = row.Field<string>("DeliveryAddress2"),
                               DeliveryAddress3 = row.Field<string>("DeliveryAddress3"),
                               VehicleType = row.Field<string>("VehicleType"),
                               VehicleNo = row.Field<string>("VehicleNo"),
                               ProductNameOld = row.Field<string>("ProductNameOld"),
                               ProductDescription = row.Field<string>("ProductDescription"),
                               ProductGroup = row.Field<string>("ProductGroup"),
                               UOM = row.Field<string>("UOM"),
                               ProductCommercialName = row.Field<string>("ProductCommercialName"),
                               VATRegistrationNo = row.Field<string>("VATRegistrationNo"),
                               SerialNo = row.Field<string>("SerialNo"),
                               AlReadyPrint = row.Field<int>("AlReadyPrint"),
                               ImportIDExcel = row.Field<string>("ImportIDExcel"),
                               Comments = row.Field<string>("Comments"),
                               VATType = row.Field<string>("VATType"),
                               LCNumber = row.Field<string>("LCNumber"),
                               LCBank = row.Field<string>("LCBank"),
                               PINo = row.Field<string>("PINo"),
                               EXPFormNo = row.Field<string>("EXPFormNo"),
                               BranchId = row.Field<int>("BranchId"),
                               SignatoryName = row.Field<string>("SignatoryName"),
                               SignatoryDesig = row.Field<string>("SignatoryDesig"),
                               SerialNo1 = row.Field<string>("SerialNo1"),
                               FileName = row.Field<string>("FileName"),
                               SaleType = row.Field<string>("SaleType"),
                               PreviousSalesInvoiceNo = row.Field<string>("PreviousSalesInvoiceNo"),
                               TransactionType = row.Field<string>("TransactionType"),
                               CurrencyID = row.Field<string>("CurrencyID"),
                               TPurchaseInvoiceNo = row.Field<string>("TPurchaseInvoiceNo"),
                               TBENumber = row.Field<string>("TBENumber"),
                               TCustomHouse = row.Field<string>("TCustomHouse"),
                               CustomerCode = row.Field<string>("CustomerCode"),
                               VATRate = row.Field<decimal>("VATRate"),

                               PreviousNBRPrice = row.Field<decimal>("PreviousNBRPrice"),
                               PreviousQuantity = row.Field<decimal>("PreviousQuantity"),
                               PreviousSubTotal = row.Field<decimal>("PreviousSubTotal"),
                               PreviousVATRate = row.Field<decimal>("PreviousVATRate"),
                               PreviousVATAmount = row.Field<decimal>("PreviousVATAmount"),
                               PreviousSD = row.Field<decimal>("PreviousSD"),
                               PreviousSDAmount = row.Field<decimal>("PreviousSDAmount"),
                               Fixed_Subtotal = row.Field<decimal>("Fixed_Subtotal"),
                               PreLineTotal = row.Field<decimal>("PreLineTotal"),
                               LineTotal = row.Field<decimal>("LineTotal"),

                               ReasonOfReturn = row.Field<string>("ReasonOfReturn"),
                               PreviousSalesInvoiceNoD = row.Field<string>("PreviousSalesInvoiceNoD"),
                               PreviousInvoiceDateTime = row.Field<string>("PreviousInvoiceDateTime"),
                               PreviousUOM = row.Field<string>("PreviousUOM")
                           }
                               into grp
                               select new
                               {
                                   SalesInvoiceNo = grp.Key.SalesInvoiceNo,
                                   InvoiceDate = grp.Key.InvoiceDate,
                                   CustomerName = grp.Key.CustomerName,
                                   ProductName = grp.Key.ProductName.ToUpper(),
                                   Address1 = grp.Key.Address1,
                                   Address2 = grp.Key.Address2,
                                   Address3 = grp.Key.Address3,
                                   TelephoneNo = grp.Key.TelephoneNo,
                                   DeliveryAddress1 = grp.Key.DeliveryAddress1,
                                   DeliveryAddress2 = grp.Key.DeliveryAddress2,
                                   DeliveryAddress3 = grp.Key.DeliveryAddress3,
                                   VehicleType = grp.Key.VehicleType,
                                   VehicleNo = grp.Key.VehicleNo,
                                   ProductNameOld = grp.Key.ProductNameOld,
                                   ProductDescription = grp.Key.ProductDescription,
                                   ProductGroup = grp.Key.ProductGroup,
                                   UOM = grp.Key.UOM,
                                   ProductCommercialName = grp.Key.ProductCommercialName,
                                   VATRegistrationNo = grp.Key.VATRegistrationNo,
                                   SerialNo = grp.Key.SerialNo,
                                   AlReadyPrint = grp.Key.AlReadyPrint,
                                   ImportIDExcel = grp.Key.ImportIDExcel,
                                   Comments = grp.Key.Comments,
                                   VATType = grp.Key.VATType,
                                   LCNumber = grp.Key.LCNumber,
                                   LCBank = grp.Key.LCBank,
                                   PINo = grp.Key.PINo,
                                   EXPFormNo = grp.Key.EXPFormNo,
                                   BranchId = grp.Key.BranchId,
                                   SignatoryName = grp.Key.SignatoryName,
                                   SignatoryDesig = grp.Key.SignatoryDesig,
                                   SerialNo1 = grp.Key.SerialNo1,
                                   SaleType = grp.Key.SaleType,
                                   PreviousSalesInvoiceNo = grp.Key.PreviousSalesInvoiceNo,
                                   TransactionType = grp.Key.TransactionType,
                                   CurrencyID = grp.Key.CurrencyID,
                                   TPurchaseInvoiceNo = grp.Key.TPurchaseInvoiceNo,
                                   TBENumber = grp.Key.TBENumber,
                                   TCustomHouse = grp.Key.TCustomHouse,
                                   VATRate = grp.Key.VATRate,
                                   grp.Key.CustomerCode,
                                   Quantity = grp.Sum(r => r.Field<Decimal>("Quantity")),
                                   UnitCost = grp.Average(r => r.Field<Decimal>("UnitCost")),
                                   SDAmount = grp.Sum(r => r.Field<Decimal>("SDAmount")),
                                   VATAmount = grp.Sum(r => r.Field<Decimal>("VATAmount")),
                                   grp.Key.ReasonOfReturn,
                                   grp.Key.PreviousSalesInvoiceNoD,
                                   grp.Key.PreviousInvoiceDateTime,
                                   grp.Key.PreviousUOM,
                                   grp.Key.FileName,
                                   Sort = 0,

                                   PreviousNBRPrice = grp.Average(r => r.Field<Decimal>("PreviousNBRPrice")),
                                   PreviousSDAmount = grp.Sum(r => r.Field<Decimal>("PreviousSDAmount")),
                                   PreviousQuantity = grp.Sum(r => r.Field<Decimal>("PreviousQuantity")),
                                   PreviousSubTotal = grp.Sum(r => r.Field<Decimal>("PreviousSubTotal")),
                                   Fixed_Subtotal = grp.Sum(r => r.Field<Decimal>("Fixed_Subtotal")),
                                   PreviousVATAmount = grp.Sum(r => r.Field<Decimal>("PreviousVATAmount")),
                                   PreLineTotal = grp.Sum(r => r.Field<Decimal>("PreLineTotal")),
                                   LineTotal = grp.Sum(r => r.Field<Decimal>("LineTotal")),
                               }).ToList();

            string tt = JsonConvert.SerializeObject(newSort);

            List<DhlCreditNoteMOdel> list = JsonConvert.DeserializeObject<List<DhlCreditNoteMOdel>>(tt);

            list = list.Select(x =>
                {
                    if (x.ProductName.ToLower() == "Express Worldwide Doc".ToLower())
                    {
                        x.Sort = 1;
                    }
                    else if (x.ProductName.ToLower() == "Express Worldwide Nondoc".ToLower())
                    {
                        x.Sort = 2;
                    }
                    else if (x.ProductName.ToLower() == "Express 9:00 Nondoc".ToLower())
                    {
                        x.Sort = 3;
                    }
                    else if (x.ProductName.ToLower() == "Express 10:30 Nondoc".ToLower())
                    {
                        x.Sort = 4;
                    }
                    else if (x.ProductName.ToLower() == "Express 12:00 Nondoc".ToLower())
                    {
                        x.Sort = 5;
                    }

                    return x;
                }).OrderBy(x => x.Sort)
                .ToList();


            tt = JsonConvert.SerializeObject(list);
            tempResult = JsonConvert.DeserializeObject<DataTable>(tt);
            tempResult.Columns.Remove("Sort");

            tempResult.TableName = "DsVAT11";
            return tempResult;
        }

        private DataTable GroupByResult(DataTable tempResult)
        {
            var newSort = (from row in tempResult.AsEnumerable()
                           group row by new
                           {
                               SalesInvoiceNo = row.Field<string>("SalesInvoiceNo"),
                               InvoiceDate = row.Field<string>("InvoiceDate"),
                               CustomerName = row.Field<string>("CustomerName"),
                               ProductName = row.Field<string>("ProductName"),
                               Address1 = row.Field<string>("Address1"),
                               Address2 = row.Field<string>("Address2"),
                               Address3 = row.Field<string>("Address3"),
                               TelephoneNo = row.Field<string>("TelephoneNo"),
                               DeliveryAddress1 = row.Field<string>("DeliveryAddress1"),
                               DeliveryAddress2 = row.Field<string>("DeliveryAddress2"),
                               DeliveryAddress3 = row.Field<string>("DeliveryAddress3"),
                               VehicleType = row.Field<string>("VehicleType"),
                               VehicleNo = row.Field<string>("VehicleNo"),
                               ProductNameOld = row.Field<string>("ProductNameOld"),
                               ProductDescription = row.Field<string>("ProductDescription"),
                               ProductGroup = row.Field<string>("ProductGroup"),
                               UOM = row.Field<string>("UOM"),
                               ProductCommercialName = row.Field<string>("ProductCommercialName"),
                               VATRegistrationNo = row.Field<string>("VATRegistrationNo"),
                               SerialNo = row.Field<string>("SerialNo"),
                               AlReadyPrint = row.Field<int>("AlReadyPrint"),
                               FileName = row.Field<string>("FileName"),
                               ImportIDExcel = row.Field<string>("ImportIDExcel"),
                               Comments = row.Field<string>("Comments"),
                               VATType = row.Field<string>("VATType"),
                               LCNumber = row.Field<string>("LCNumber"),
                               LCBank = row.Field<string>("LCBank"),
                               PINo = row.Field<string>("PINo"),
                               EXPFormNo = row.Field<string>("EXPFormNo"),
                               BranchId = row.Field<int>("BranchId"),
                               SignatoryName = row.Field<string>("SignatoryName"),
                               SignatoryDesig = row.Field<string>("SignatoryDesig"),
                               SerialNo1 = row.Field<string>("SerialNo1"),
                               SaleType = row.Field<string>("SaleType"),
                               PreviousSalesInvoiceNo = row.Field<string>("PreviousSalesInvoiceNo"),
                               TransactionType = row.Field<string>("TransactionType"),
                               CurrencyID = row.Field<string>("CurrencyID"),
                               TPurchaseInvoiceNo = row.Field<string>("TPurchaseInvoiceNo"),
                               TBENumber = row.Field<string>("TBENumber"),
                               TCustomHouse = row.Field<string>("TCustomHouse"),
                               CustomerCode = row.Field<string>("CustomerCode"),
                               VATRate = row.Field<decimal>("VATRate"),
                               SD = row.Field<decimal>("SD"),
                               //Fixed_Subtotal = row.Field<decimal>("Fixed_Subtotal")
                           }
                               into grp
                               select new
                               {
                                   SalesInvoiceNo = grp.Key.SalesInvoiceNo,
                                   InvoiceDate = grp.Key.InvoiceDate,
                                   CustomerName = grp.Key.CustomerName,
                                   ProductName = grp.Key.ProductName.ToUpper(),
                                   Address1 = grp.Key.Address1,
                                   Address2 = grp.Key.Address2,
                                   Address3 = grp.Key.Address3,
                                   TelephoneNo = grp.Key.TelephoneNo,
                                   DeliveryAddress1 = grp.Key.DeliveryAddress1,
                                   DeliveryAddress2 = grp.Key.DeliveryAddress2,
                                   DeliveryAddress3 = grp.Key.DeliveryAddress3,
                                   VehicleType = grp.Key.VehicleType,
                                   VehicleNo = grp.Key.VehicleNo,
                                   ProductNameOld = grp.Key.ProductNameOld,
                                   ProductDescription = grp.Key.ProductDescription,
                                   ProductGroup = grp.Key.ProductGroup,
                                   UOM = grp.Key.UOM,
                                   ProductCommercialName = grp.Key.ProductCommercialName,
                                   VATRegistrationNo = grp.Key.VATRegistrationNo,
                                   SerialNo = grp.Key.SerialNo,
                                   AlReadyPrint = grp.Key.AlReadyPrint,
                                   ImportIDExcel = grp.Key.ImportIDExcel,
                                   Comments = grp.Key.Comments,
                                   VATType = grp.Key.VATType,
                                   LCNumber = grp.Key.LCNumber,
                                   LCBank = grp.Key.LCBank,
                                   PINo = grp.Key.PINo,
                                   EXPFormNo = grp.Key.EXPFormNo,
                                   BranchId = grp.Key.BranchId,
                                   SignatoryName = grp.Key.SignatoryName,
                                   SignatoryDesig = grp.Key.SignatoryDesig,
                                   SerialNo1 = grp.Key.SerialNo1,
                                   SaleType = grp.Key.SaleType,
                                   PreviousSalesInvoiceNo = grp.Key.PreviousSalesInvoiceNo,
                                   TransactionType = grp.Key.TransactionType,
                                   CurrencyID = grp.Key.CurrencyID,
                                   TPurchaseInvoiceNo = grp.Key.TPurchaseInvoiceNo,
                                   TBENumber = grp.Key.TBENumber,
                                   TCustomHouse = grp.Key.TCustomHouse,
                                   VATRate = grp.Key.VATRate,
                                   SD = grp.Key.SD,
                                   grp.Key.CustomerCode,
                                   grp.Key.FileName,
                                   Quantity = grp.Sum(r => r.Field<Decimal>("Quantity")),
                                   UnitCost = grp.Average(r => r.Field<Decimal>("UnitCost")),
                                   SDAmount = grp.Sum(r => r.Field<Decimal>("SDAmount")),
                                   VATAmount = grp.Sum(r => r.Field<Decimal>("VATAmount")),
                                   Fixed_Subtotal = grp.Sum(r => r.Field<Decimal>("Fixed_Subtotal")),
                                   LineTotal = grp.Sum(r => r.Field<Decimal>("LineTotal")),
                                   Sort = 0
                               }).ToList();

            string tt = JsonConvert.SerializeObject(newSort);

            List<SaleDHLReport> list = JsonConvert.DeserializeObject<List<SaleDHLReport>>(tt);

            list = list.Select(x =>
                {
                    if (x.ProductName.ToLower() == "Express Worldwide Doc".ToLower())
                    {
                        x.Sort = 1;
                    }
                    else if (x.ProductName.ToLower() == "Express Worldwide Nondoc".ToLower())
                    {
                        x.Sort = 2;
                    }
                    else if (x.ProductName.ToLower() == "Express 9:00 Nondoc".ToLower())
                    {
                        x.Sort = 3;
                    }
                    else if (x.ProductName.ToLower() == "Express 10:30 Nondoc".ToLower())
                    {
                        x.Sort = 4;
                    }
                    else if (x.ProductName.ToLower() == "Express 12:00 Nondoc".ToLower())
                    {
                        x.Sort = 5;
                    }

                    return x;
                }).OrderBy(x => x.Sort)
                .ToList();


            tt = JsonConvert.SerializeObject(list);
            tempResult = JsonConvert.DeserializeObject<DataTable>(tt);
            tempResult.Columns.Remove("Sort");
            return tempResult;
        }

        private void ExportPDf(ReportDocument objrpt, string fileDirectory, string fileName)
        {
            Stream st = new MemoryStream();

            st = objrpt.ExportToStream(ExportFormatType.PortableDocFormat);
            string path = fileDirectory + "\\" + fileName + ".pdf";

            using (var output = new FileStream(path, FileMode.Create))
            {
                st.CopyTo(output);
            }

            st.Dispose();
        }

        private void bgwReportGeneration_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            isProcessRunning = false;
            //MessageBox.Show("Pdf Generated Completed");
        }

        private void DeleteFiles(string fileDirectory)
        {
            DirectoryInfo di = new DirectoryInfo(fileDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        private void MoveToFolder(string source, string dest)
        {

            List<String> files = Directory
                .GetFiles(source).ToList();

            foreach (string file in files)
            {
                if (IsFileLocked(file))
                    continue;

                string finalDest = dest + "\\" + Path.GetFileName(file);
                MoveFile(file, finalDest);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string path = "";
            path = chkIBS.Checked ? ibsPath : iasPath;

            progressBar1.Visible = true;

            string[] filesPosted = Directory.GetFiles(Directory.GetParent(path) + "\\Posted");
            string[] filesHold = Directory.GetFiles(Directory.GetParent(path) + "\\Hold");
            string[] filesSkipped = Directory.GetFiles(Directory.GetParent(path) + "\\Skipped");
            string[] work = Directory.GetFiles(path);

            string[] files = new string[filesPosted.Length + filesHold.Length + work.Length + filesSkipped.Length];


            try
            {
                Array.Copy(filesPosted, files, filesPosted.Length);
                //Array.Copy(filesHold, 0, files, filesPosted.Length, filesHold.Length);
                //Array.Copy(work, 0, files, filesHold.Length + filesPosted.Length, work.Length);
                Array.Copy(filesSkipped, 0, files, filesPosted.Length, filesSkipped.Length);


                string[] ids = txtId.Text.Split(',');
                int foundCount = 0;



                int totalSearched = 0;

                for (var index = 0; index < files.Length; index++)
                {
                    try
                    {
                        string file = files[index];

                        if (IsFileLocked(file))
                            continue;

                        string fileName = Path.GetFileName(file);
                        string currentFilePath = file;

                        if (true)
                        {
                            using (FileStream zipFile = File.Open(file, FileMode.Open))
                            {
                                using (ZipArchive archive = new ZipArchive(zipFile))
                                {
                                    //fileName = fileName.Replace(".zip", "");

                                    ZipArchiveEntry
                                        entry = archive.Entries.FirstOrDefault(); //archive.GetEntry(fileName);

                                    using (MemoryStream memory = new MemoryStream())
                                    {
                                        entry.Open().CopyTo(memory);
                                        memory.Seek(0, SeekOrigin.Begin);
                                        StreamReader streamReader = new StreamReader(memory);
                                        string xml = streamReader.ReadToEnd();

                                        streamReader.Close();
                                        memory.Close();
                                        zipFile.Close();

                                        string name = Path.GetFileName(file);
                                        totalSearched++;
                                        foreach (string id in ids)
                                        {
                                            if (xml.Contains(id))
                                            {
                                                FileLogger.Log("DHL", "Getfiles", name + "---" + id + "\n" + " Location : " + file);
                                                foundCount++;
                                            }
                                        }

                                        if (foundCount == ids.Length)
                                        {
                                            break;
                                        }

                                    }
                                }
                            }
                        }


                    }
                    catch (Exception ex)
                    {


                        FileLogger.Log("DHL", "Getfiles", ex.Message + "\n" + ex.StackTrace);
                    }
                }


                FileLogger.Log("DHL", "Search Files", "Total Files " + files.Length + "\n" + "Total Searched " + totalSearched + "\n");

                MessageBox.Show("Complete");

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

        private void SaveAirport()
        {
            try
            {
                UserInformationDAL uDal = new UserInformationDAL();
                ImportDAL importDal = new ImportDAL();
                SaleDAL salesDal = new SaleDAL();
                BranchProfileDAL dal = new BranchProfileDAL();

                DataTable dtBranchInfo = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);


                DataTable salesData = importDal.GetSaleDHLAirportDbData("", dtBranchInfo, "", connVM);//, uVm.FullName


                FileLogger.Log("From Sale Import DHL", "BgwAirport",
                    "Before Save : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Number Of" + salesData.Rows.Count);

                if (salesData.Rows.Count != 0)
                {
                    #region TableValidation and date Save

                    TableValidationAirport(salesData);

                    IntegrationParam param = new IntegrationParam();


                    sqlResults = salesDal.SaveAndProcess(salesData, () => { }, Program.BranchId, "", connVM, param,
                        null, null, Program.CurrentUserID);


                    #endregion

                    #region Update OtherDB

                    FileLogger.Log("From Sale Import DHL", "timer1_Tick",
                        "After Save : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + sqlResults[0].ToLower());

                    if (sqlResults[0].ToLower() == "success")
                    {
                        UpdateAirportDB(salesData, transactionType);
                    }

                    #endregion

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log("From Sale Import DHL", "SaveAirport", "SaveAirport Error : " + ex.ToString());

            }
        }

        private void TableValidationAirport(DataTable salesData)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = Program.BranchCode };
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            ////var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = Program.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = transactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            salesData.Columns.Add(column);
            //salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);

            if (!salesData.Columns.Contains("TransactionType"))
            {
                salesData.Columns.Add(TransactionType);
            }
        }

        private void UpdateAirportDB(DataTable salesData, string transactionType = "Other")
        {
            if (sqlResults[0].ToLower() == "success")
            {
                try
                {
                    ImportDAL importDal = new ImportDAL();

                    DataView view = new DataView(salesData);

                    salesData = view.ToTable(true, "ID");

                    string[] results = new string[5];

                    results = importDal.UpdateDHLSales(salesData, settingVM.BranchInfoDT, connVM);

                    if (results[0].ToLower() != "success")
                    {
                        string message = "These Id failed to insert to other database\n";

                        foreach (DataRow row in salesData.Rows)
                        {
                            message += row["ID"] + "\n" + " , ";
                        }

                        FileLogger.Log("DHL", "UpdateOtherDB", message);

                    }

                    FileLogger.Log("From Sale Import DHL", "timer1_Tick",
                        "After Updating Middleware : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " +
                        sqlResults[0].ToLower());
                }
                catch (Exception e)
                {
                    string message = "These Id failed to insert to other database\n";

                    foreach (DataRow row in salesData.Rows)
                    {
                        message += row["ID"] + "\n" + " , ";
                    }

                    FileLogger.Log("DHL", "UpdateOtherDB", message);

                }
            }
        }

        private void bgwAirport_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                SaveAirport();
            }
            catch (Exception ex)
            {
                FileLogger.Log("From Sale Import DHL", "bgwAirport_DoWork", "bgwAirport_DoWork Error : " + ex.ToString());
            }

        }

        private void bgwAirport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                FileLogger.Log("Bgw CRAT Complete", "Bgw CRAT Complete", e.ToString());

            }
            else
            {
                FileLogger.Log("Bgw CRAT Complete", "Bgw CRAT Complete", DateTime.Now.ToString());

            }

            isProcessRunning = false;
            progressBar1.Visible = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        public void EmailProcess(string InvoiceNo, string fileDirectory, string fileName)
        {
            try
            {
                SaleDAL _sDal = new SaleDAL();
                MailSettings ems = new MailSettings();
                CommonDAL _setDAL = new CommonDAL();
                ems.MailHeader = _setDAL.MailsettingValue("Mail", "MailSubject");
                ////ems.MailHeader = ems.MailHeader.Replace("vmonth", FiscalPeriod);
                string mailbody = _setDAL.MailsettingValue("Mail", "MailBody");

                try
                {
                    string CustomerEmail = _sDal.SelectCustomerEmail(InvoiceNo);

                    string filePath = fileDirectory + "\\" + fileName + ".pdf";

                    ////ems.MailToAddress = CustomerEmail;

                    ems.MailToAddress = "alamgir.hossain@symphonysoftt.com";
                    ems.Port = 25;
                    ems.ServerName = "SMTP.britishcouncil.org";//smtp.gmail.com

                    if (!string.IsNullOrWhiteSpace(ems.MailToAddress))
                    {
                        ems.MailBody = mailbody;

                        ////ems.MailBody = mailbody.Replace("vmonth", FiscalPeriod);
                        ////ems.MailBody = mailbody.Replace("vname", item["EmpName"].ToString());

                        using (var smpt = new SmtpClient())
                        {
                            smpt.EnableSsl = ems.USsel;
                            smpt.Host = ems.ServerName;
                            smpt.Port = ems.Port;
                          
                            smpt.UseDefaultCredentials = false;
                            smpt.EnableSsl = true;
                            smpt.Credentials = new NetworkCredential(ems.UserName, ems.Password);
                            MailMessage mailmessage = new MailMessage(
                                ems.MailFromAddress,
                                ems.MailToAddress,
                                ems.MailHeader,
                                ems.MailBody);
                            mailmessage.Attachments.Add(new Attachment(filePath));

                            smpt.Send(mailmessage);
                            mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                            string result = _sDal.UpdateIsSendMail(InvoiceNo);

                        }

                        Thread.Sleep(500);
                    }
                }
                catch (SmtpFailedRecipientException ex)
                {
                    // throw ex;
                }

                //rptDoc.Close();
                //thread.Abort();

            }
            catch (Exception ex)
            {
                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = "0";
                    evm.FileName = "MailSend";
                    evm.ErrorMassage = ex.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "EmailProcess";
                    evm.ActionName = "FormSaleImportBC";
                    evm.TransactionName = "Sales";

                    CommonDAL _cDal = new CommonDAL();

                    string[] Logresult = _cDal.InsertErrorLogs(evm);

                }
                catch (Exception exc)
                {

                }
            }

        }

    }
}
