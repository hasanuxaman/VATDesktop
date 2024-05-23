using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
//using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using Microsoft.SqlServer.Management.Common;
using VATDesktop.Repo;
using VATServer.Interface;

namespace VATClient
{
    public partial class FormSaleImportSQR : Form
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

        private string _saleRow = "~N";
        private string isExempted = "N";

        private string _selectedDb = "Link3_Demo_DB";

        public bool IsCDN = false;

        #endregion
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public FormSaleImportSQR()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        public static string SelectOne(string transactionType, bool cdn)
        {
            FormSaleImportSQR form = new FormSaleImportSQR();

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

            if (transactionType.ToLower() == "export")
            {
                chkInstitution.Visible = false;
                chkInstitution.Checked = false;
            }


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
                    var importDal =   new ImportDAL();// OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo,IImport>(Program.IsWCF);

                    selectedType = cmbImportType.Text;


                    var saleDal = new SaleDAL();

                    var invoiceNo = txtId.Text.Trim();

                    if (string.IsNullOrEmpty(invoiceNo))
                    {
                        MessageBox.Show(@"Please Enter Transaction No");
                        return;
                    }


                    var ids = invoiceNo.Split(',');

                    string value = _cDal.settingValue("Import", "SaleExistContinue",connVM);

                    if (value == "N")
                    {
                        List<SaleMasterVM> sales = saleDal.SelectAllList(0, new[] { "sih.ImportIDExcel" }, new[] { invoiceNo }, null, null, null,connVM);

                        if (sales != null && sales.Count > 0)
                        {
                            var sale = sales.FirstOrDefault();

                            if (sale != null)
                                MessageBox.Show(@"This Transaction No is already in system with invoice no - " +
                                                sale.SalesInvoiceNo);

                            return;
                        }
                    }



                    var dal = OrdinaryVATDesktop.GetObject<BranchProfileDAL, BranchProfileRepo, IBranchProfile>(Program.IsWCF);//new BranchProfileDAL();// OrdinaryVATDesktop.GetObject<BranchProfileDAL, BranchProfileRepo, IBranchProfile>(Program.IsWCF);//

                    var dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true, connVM);

                    var branchCode = dt.Rows[0]["IntegrationCode"].ToString();

                    if (dt.Rows[0]["DbType"].ToString() == "oracle")
                    {
                        if (branchCode != "UEN")
                        {
                            if (branchCode.ToLower() == "SFBL_DEN".ToLower())
                            {
                                //FileLogger.Log("FormSaleImportSQR", "btnLoad_Click", "DINAJPUR branch integration start");

                                dtTableResult = importDal.GetSaleSQRDbData_DINAJPUR(ids, dt, invoiceNo);
                            }
                            else
                            {
                                dtTableResult = importDal.GetSaleSQRDbData(ids, dt, invoiceNo);
                            }
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
                                   temp =  importDal.GetSaleSQRDbDataSenora(new[] {id.TrimStart('H')}, dt, invoiceNo);
                                }
                                else if(prefix == 'S')
                                { 
                                    temp = importDal.GetSaleSQRDbDataSoap(new[] { id.TrimStart('S') }, dt, invoiceNo);
                                }


                                table.Merge(temp);
                            }

                            dtTableResult = table.Copy();
                        }
                    }
                    else if (dt.Rows[0]["DbType"].ToString() == "sql")
                    {
                        dtTableResult = importDal.GetSaleSQR_ADH_Data(ids, dt, invoiceNo);
                        //dtTableResult = importDal.GetSaleAduriDbData(ids, dt, invoiceNo);
                    }
                    dgvLoadedTable.DataSource = dtTableResult;
                    //dtTableResult.Columns.Remove("IsVATComplete");


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

       

        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var salesDal = new SaleDAL();

                if (selectedType == CONST_ProcessOnly)
                {
                    sqlResults = salesDal.SaveAndProcessTempData(new DataTable(), BulkCallBack, Program.BranchId,connVM);
                }
                else
                {

                    #region Update Vat Rate and Price

                    var filterdData = new DataTable();

                    foreach (DataColumn column in dtTableResult.Columns)
                    {
                        filterdData.Columns.Add(column.ColumnName);
                    }


                    foreach (DataRow row in dtTableResult.Rows)
                    {

                        var currentRows = filterdData.Select("Item_Code= '" + row["Item_Code"] +
                                                             "' and Customer_Code= '" + row["Customer_Code"] + "'");

                        if (currentRows.Length == 0)
                        {
                            var rows = dtTableResult.Select("Item_Code= '" + row["Item_Code"] +
                                                            "' and Customer_Code= '" + row["Customer_Code"] + "'");

                            decimal quantity = 0;
                            decimal subTotal = 0;
                            foreach (DataRow dataRow in rows)
                            {
                                quantity += Convert.ToDecimal(dataRow["Quantity"]);
                                subTotal += Convert.ToDecimal(dataRow["SubTotal"]);
                            }

                            row["Quantity"] = quantity;
                            row["SubTotal"] = subTotal;

                            var items = row.ItemArray;

                            filterdData.Rows.Add(items);
                        }

                    }


                    var salesData = filterdData;
                    ProductDAL pDal = new ProductDAL();
                    //var pDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(Program.IsWCF); //new ProductDAL();  //OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(Program.IsWCF);//

                    string customerCode = salesData.Rows[0]["Customer_Code"].ToString().Trim();
                    string customerName = salesData.Rows[0]["Customer_Name"].ToString().Trim();
                    string deliveryAddress = salesData.Rows[0]["Delivery_Address"].ToString().Trim('\r', '\n').Trim();
                    string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");



                    #region Get Institution Flag

                    //CustomerDAL customerDal = new CustomerDAL();

                    //List<CustomerVM> customerVMs =
                    //    customerDal.SelectAllList(null, new[] { "CustomerCode" }, new[] { customerCode });


                    //if (!customerVMs.Any())
                    //{
                    //    throw new Exception("Customer Not Found");
                    //}

                    //bool isInstitution = customerVMs.FirstOrDefault().IsInstitution == "Y";

                    //chkInstitution.Checked = isInstitution;



                    #endregion



                    CustomerDAL customerDAL = new CustomerDAL();

                    CustomerVM customer = customerDAL.SelectAllList(null, new[] {"CustomerCode"}, new[] {customerCode},null,null,connVM).FirstOrDefault();

                    if (customer != null)
                    {
                        isExempted = customer.IsExamted;
                    }

                    foreach (DataRow row in salesData.Rows)
                    {
                        List<ProductVM> vms = pDal.SelectAll("0", new[] { "ProductCode" }, new[] { row["Item_Code"].ToString() }, null, null,null, connVM);

                        row["Delivery_Date_Time"] = time;
                        row["Invoice_Date_Time"] = time;

                        if (vms != null && vms.Any())
                        {
                            var vm = vms.FirstOrDefault();
                            decimal sd = Convert.ToDecimal(string.Format("{0:0.0000}", vm.SD));
                            decimal vRate = Convert.ToDecimal(string.Format("{0:0.0000}", vm.VATRate));

                            decimal subtotal = Convert.ToDecimal(string.Format("{0:0.0000}", row["SubTotal"]));
                            decimal quantity = Convert.ToDecimal(string.Format("{0:0.0000}", row["Quantity"]));

                            if (!chkInstitution.Checked)
                            {
                                row["NBR_Price"] = vm.NBRPrice;
                                row["SubTotal"] = "0";
                            }
                            else
                            {

                                //if (Program.BranchCode.ToLower() == "adu" || Program.BranchCode.ToLower() == "ctg")
                                //{
                                //    if (string.IsNullOrEmpty(txtPercentage.Text))
                                //    {
                                //        throw new Exception("Please enter value in percentage");
                                //    }

                                //    decimal orgaPercent = Convert.ToDecimal(txtPercentage.Text);
                                //    decimal orgaPrice = Convert.ToDecimal(row["NBR_Price"]);


                                //  //  orgaPrice = orgaPrice - (orgaPrice * (orgaPercent / 100));

                                //    orgaPrice = NBRPriceCal(sd, vRate, orgaPrice, 1);

                                //    row["NBR_Price"] = orgaPrice;

                                //}
                                //else if (Program.BranchCode.ToLower() != "adu")
                                //{
                                //    row["NBR_Price"] = NBRPriceCal(sd, vRate, subtotal, quantity);

                                //}


                                row["NBR_Price"] = NBRPriceCal(sd, vRate, subtotal, quantity);
                                row["SubTotal"] = "0";
                            }



                            row["VAT_Rate"] = vm.VATRate;
                            row["UOM"] = vm.UOM;
                            row["SD_Rate"] = vm.SD;


                            if (vm.VATRate == 15)
                            {
                                row["Type"] = "VAT";
                            }

                            if (row["VAT_Rate"].ToString() == "0")
                            {
                                row["Type"] = "NonVAT";

                            }

                            if (vm.VATRate != 15 && vm.VATRate !=0)
                            {
                                row["Type"] = "OtherRate";
                            }


                            if (string.IsNullOrEmpty(row["Type"].ToString()))
                            {
                                row["Type"] = "NonVAT";
                            }


                            if (transactionType.ToLower() == "export")
                            {
                                row["Type"] = "Export";
                                row["VAT_Rate"] = 0;
                                row["UOM"] = vm.UOM;
                                row["SD_Rate"] = 0;
                                row["NBR_Price"] = NBRPriceCal(0, 0, subtotal, quantity);
                            }

                            //row["SubTotal"] = vms.FirstOrDefault().NBRPrice * Convert.ToInt32(row["Quantity"]);
                        }

                        if (isExempted.ToLower() == "y")
                        {
                            row["Type"] = "NonVAT";
                            row["VAT_Rate"] = 0;
                            row["SD_Rate"] = 0;
                        }

                        row["Delivery_Address"] = deliveryAddress;
                        row["Customer_Name"] = customerName;

                        if (customerCode != row["Customer_Code"].ToString().Trim())
                        {
                            throw new Exception("Multiple customer can not be add to a single invoice");
                        }

                    }

                    #endregion

                    TableValidation(salesData);

                    if (InvokeRequired)
                        Invoke((MethodInvoker)delegate { PercentBar(5); });

                    BulkCallBack();

                    //sqlResults = salesDal.SaveAndProcess(salesData, BulkCallBack, Program.BranchId);

                    sqlResults = salesDal.SaveAndProcessIntegration(salesData, BulkCallBack, Program.BranchId, Program.GetAppVersion(),connVM, Program.CurrentUserID);



                }

                

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
        }

        private decimal NBRPriceCal(decimal sd, decimal vatRate, decimal subtotal, decimal quantity)
        {
            decimal vatAbleValue = (subtotal * 100) / (100 + vatRate);

            decimal sdAbleValue = (vatAbleValue * 100) / (100 + sd);

            decimal nbrPrice = (sdAbleValue / quantity);


            string Pre = "";
            Pre = Pre.PadRight(4, '#');

            nbrPrice = decimal.Parse(nbrPrice.ToString("####0." + Pre), System.Globalization.NumberStyles.Float);


            return nbrPrice;
        }

        private void TableValidation(DataTable salesData)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") {DefaultValue = Program.BranchCode};
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") {DefaultValue = ""};
            var CreatedBy = new DataColumn("CreatedBy") {DefaultValue = Program.CurrentUser};
            var ReturnId = new DataColumn("ReturnId") {DefaultValue = 0};
            var BOMId = new DataColumn("BOMId") {DefaultValue = 0};
            var TransactionType = new DataColumn("TransactionType") {DefaultValue = transactionType};
            var CreatedOn = new DataColumn("CreatedOn") {DefaultValue = DateTime.Now.ToString()};
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

        }


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

        private void dgvLoadedTable_DoubleClick(object sender, EventArgs e)
        {
          
        }

        private void dgvLoadedTable_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            _saleRow = dgvLoadedTable.CurrentRow.Cells["Id"].Value.ToString() + "~"+isExempted;
            this.Hide();


        }

        private void dgvLoadedTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chkInstitution_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (Program.BranchCode.ToLower() == "adu" || Program.BranchCode.ToLower() == "ctg")
                //{
                //    label1.Visible = chkInstitution.Checked;
                //    txtPercentage.Visible = chkInstitution.Checked;
                //}
                
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        

    }
}
