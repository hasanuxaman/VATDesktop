using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
//using Microsoft.SqlServer.Management.Common;

using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VMSAPI;


namespace VATClient
{
    public partial class FormSaleImportJapha : Form
    {
        #region Variables
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
        private const string CONST_XML = "XML";
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

        private string _saleRow = "";

        private string _selectedDb = "Link3_Demo_DB";

        public bool IsCDN = false;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        #endregion

        public FormSaleImportJapha()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType)
        {
            FormSaleImportJapha form = new FormSaleImportJapha();


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
            cmbImportType.Items.Add(CONST_XML);
            cmbImportType.Items.Add(CONST_EXCEL);
            cmbImportType.Items.Add(CONST_TEXT);
            cmbImportType.Items.Add(CONST_DATABASE);
            cmbImportType.Items.Add(CONST_ProcessOnly);

            CommonDAL dal = new CommonDAL();

            string value = dal.settingsDesktop("Import", "SaleImportSelection",null,connVM);

            cmbImportType.SelectedItem = CONST_XML;

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
                switch (selectedType)
                {
                    case CONST_EXCEL:
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
                        break;
                    }
                    case CONST_TEXT:
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

                        break;
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
                    //dtTableResult = OrdinaryVATDesktop.dt
                    dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new[] { "Invoice_Date_Time", "Delivery_Date_Time" });
                    loadedTable = CONST_SALETYPE;

                }
                #endregion
                #region Text
                else if (selectedType == CONST_TEXT)
                {

                    var flag = new CommonDAL().settingValue(CONST_SINGLEIMPORT, "SaleImport",connVM);

                    if (flag == "Y")
                    {
                        dtTableResult = GetTableFromText(CONST_SALETYPE);
                        //dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_SALETYPE;
                    }
                    else
                    {
                        dtTableResult = GetTableFromTextDouble();
                        //dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = CONST_SALETYPE;
                    }

                }
                #endregion
                #region XML
                else if (selectedType == CONST_XML)
                {
                    ds = LoadFromXML();


                    dtTableResult = ds.Tables["SaleM"];
                    //dtTableResult = OrdinaryVATDesktop.dt
                    dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtTableResult, new[] { "Invoice_Date_Time", "Delivery_Date_Time" });
                    loadedTable = CONST_SALETYPE;

                }
                #endregion

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
                    var importDal = new ImportDAL();

                    selectedType = cmbImportType.Text;
                    string[] cFields = new string[] { "IsVATComplete", "ID" };
                    string[] cVals = new string[] { "N", txtId.Text };


                    if (chkOldDb.Checked)
                    {
                        dtTableResult = importDal.GetSaleCDNData(txtId.Text.Trim(), cmbDBList.Text.Trim('[', ']'),connVM);
                        loadedTable = CONST_SALETYPE;
                        dgvLoadedTable.DataSource = dtTableResult;

                    }
                    //else
                    //{
                    //    dtTableResult = importDal.GetSaleOtherDbData(txtId.Text.Trim());
                    //    dgvLoadedTable.DataSource = dtTableResult;
                    //}


                    //dtTableResult.Columns.Remove("IsVATComplete");


                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (dtTableResult != null)
            {
                lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

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
                else if (cmbImportType.Text == CONST_XML)
                {
                    fdlg.Filter = "XML Files (*.xml)|*.xml";
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
                //SaleAPI api = new SaleAPI();
                //api.InsertSale()

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

        private DataSet LoadFromXML()
        {
            DataSet dataSet = new DataSet();

            string files = Program.ImportFileName;
         
            if (string.IsNullOrEmpty(files))
            {
                throw new Exception("There are no files to read");
            }

            string[] fileNames = files.Split(";".ToCharArray());

            using (StreamReader sr = new StreamReader(fileNames[0]))
            {
                try
                {
                    string XML = sr.ReadToEnd();
                    DataTable dtTable = new DataTable();
                    dtTable = PopulateMinimumColumn(dtTable);
                    dtTable.TableName = "SaleM";

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(XML);

                    var voucher = xmlDocument.SelectSingleNode("//VOUCHER");

               
                    string ID = voucher.SelectSingleNode("VOUCHERNUMBER").InnerText;
                    string Reference_No = voucher.SelectSingleNode("REFERENCE").InnerText;
                    string Customer_Name = voucher.SelectSingleNode("PARTYNAME").InnerText;
                    string Invoice_Date_Time = voucher.SelectSingleNode("BASICDATETIMEOFINVOICE").InnerText.Replace("at", "");

                    string Customer_Code = "-";
                    string Sale_Type = "new";
                    string Post = "N";
                    string Currency_Code = "BDT";

                    string Delivery_Address = "";
                    XmlNodeList addressList = voucher.SelectNodes("BASICBUYERADDRESS.LIST");
                    foreach (XmlNode node in addressList)
                    {
                        Delivery_Address += node.InnerText + "\n";
                    }

                    XmlNodeList lineItems = voucher.SelectNodes("ALLINVENTORYENTRIES.LIST");

                    for (var index = 0; index < lineItems.Count; index++)
                    {
                        XmlNode item = lineItems[index];

                        dtTable.Rows.Add(dtTable.NewRow());

                        dtTable.Rows[index]["ID"] = ID;
                        dtTable.Rows[index]["Reference_No"] = Reference_No;
                        dtTable.Rows[index]["Customer_Name"] = Customer_Name;
                        dtTable.Rows[index]["Customer_Code"] = Customer_Code;
                        dtTable.Rows[index]["Invoice_Date_Time"] = Invoice_Date_Time;
                        dtTable.Rows[index]["Sale_Type"] = Sale_Type;
                        dtTable.Rows[index]["Post"] = Post;
                        dtTable.Rows[index]["Currency_Code"] = Currency_Code;
                        dtTable.Rows[index]["Delivery_Address"] = Delivery_Address;
                        dtTable.Rows[index]["CustomerAddress"] = Delivery_Address;


                        dtTable.Rows[index]["Item_Code"] = item.SelectSingleNode("STOCKITEMNAME").InnerText.Split('-')[0];
                        dtTable.Rows[index]["Item_Name"] = item.SelectSingleNode("STOCKITEMNAME").InnerText.Split('-')[1];

                        dtTable.Rows[index]["Quantity"] = Regex.Replace(item.SelectSingleNode("ACTUALQTY").InnerText,
                            "[^0-9.+-]", "").Trim();

                        dtTable.Rows[index]["NBR_Price"] = item.SelectSingleNode("RATE").InnerText.Split('/')[0].Trim();
                        dtTable.Rows[index]["UOM"] = item.SelectSingleNode("RATE").InnerText.Split('/')[1].Trim();


                        dtTable.Rows[index]["VAT_Rate"] = "0"; // will be calculated based on master data
                        dtTable.Rows[index]["SD_Rate"] = "0"; // will be calculated based on master data
                        dtTable.Rows[index]["Type"] = "NonVAT"; // will be calculated based on master data
                        dtTable.Rows[index]["CommentsD"] = "-";
                        dtTable.Rows[index]["ReasonOfReturn"] = "-";

                        dtTable.Rows[index]["Previous_Invoice_No"] =
                            "";
                        dtTable.Rows[index]["PreviousInvoiceDateTime"] =
                            "1900-01-01";
                    }

                    dataSet.Tables.Add(dtTable);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
         
          
            return dataSet;
        }

        private DataTable PopulateMinimumColumn(DataTable table)
        {
            table.Rows.Clear();
            table.Columns.Clear();


            table.Columns.Add("ID");
            table.Columns.Add("Customer_Name");

            table.Columns.Add("Customer_Code");
            table.Columns.Add("Delivery_Address");
            table.Columns.Add("CustomerAddress");
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
            table.Columns.Add("Type");
            table.Columns.Add("CommentsD");
            //table.Columns.Add("CustomerBIN");
            //table.Columns.Add("Branch_Code");
            table.Columns.Add("ReasonOfReturn");
            //table.Columns.Add("DataSource");

            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "SubTotal" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "VAT_Amount" });
            table.Columns.Add(new DataColumn { DefaultValue = "Other", ColumnName = "TransactionType" });
            table.Columns.Add(new DataColumn { DefaultValue = "", ColumnName = "Previous_Invoice_No" });
            table.Columns.Add(new DataColumn { DefaultValue = "1900-01-01", ColumnName = "PreviousInvoiceDateTime" });
            table.Columns.Add(new DataColumn { DefaultValue = "N", ColumnName = "Is_Print" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "Trading_MarkUp" });
            table.Columns.Add(new DataColumn { DefaultValue = "N", ColumnName = "Non_Stock" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "Weight" });
            table.Columns.Add(new DataColumn { DefaultValue = Program.BranchCode, ColumnName = "Branch_Code" });
            table.Columns.Add(new DataColumn { DefaultValue = "VAT 4.3", ColumnName = "VAT_Name" });




            return table;
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
                sqlResults = saleDal.ImportData(dtSaleM, dtSaleD, dtSaleE, CommercialImporter,0,connVM);
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

                DataTable salesData = dtTableResult.Copy();

                TableValidation(salesData);

                SaleDAL saleDal = new SaleDAL();

                sqlResults = saleDal.SaveAndProcess(salesData, BulkCallBack, Program.BranchId, "", connVM);

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
        }

        private void SetSteps(int steps = 4)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { PercentBar(steps); });
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

                // MessageBox.Show(this, (_timePassedInMs / 1000).ToString());
                //if (_saleRowCount == 0)
                //{
                //    MessageBox.Show(this, "No Unprocessed Data Left");
                //    return;
                //}

                //if (_noBranch != null && _noBranch.Rows.Count > 0)
                //{
                //    var view = new DataView(_noBranch);

                //    var temp = view.ToTable(false, "ID", "Customer_Name", "Customer_Code", "Delivery_Address", "Invoice_Date_Time",
                //    "Delivery_Date_Time", "Item_Code", "Item_Name", "Quantity", "NBR_Price", "UOM", "Total_Price", "VAT_Rate", "Vehicle_No", "Reference_No",
                //    "Comments", "Sale_Type", "Previous_Invoice_No", "Is_Print", "Tender_Id", "Post", "LC_Number", "Currency_Code", "SD_Rate", "Non_Stock", "Trading_MarkUp", "Type",
                //    "Discount_Amount", "Promotional_Quantity", "VAT_Name", "ExpDescription", "ExpQuantity", "ExpGrossWeight", "ExpNetWeight",
                //    "ExpNumberFrom", "ExpNumberTo", "Branch_Code");

                //    dgvLoadedTable.DataSource = temp;
                //    _isDeleteTemp = false;
                //    MessageBox.Show(this, "For some data branch code does not exist. Please correct those.");
                //}

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
                        var result = new CommonDAL().UpdateIsVATComplete(loadedTable, null, null,connVM);
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

                if (!_isDeleteTemp)
                {
                    MessageBox.Show(this, "Please select new excel file");
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
            //if (cmbImportType.Text == CONST_DATABASE)
            //{

            //    //var selectedRows = dgvLoadedTable.Rows[e.RowIndex];

            //    //var firstRow = selectedRows;

            //    //var invoiceNo = firstRow.Cells[0].Value.ToString();

            //    //var saleDal = new SaleDAL();

            //    //_saleRow = saleDal.GetSaleJoin(invoiceNo);

            //}
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

    }
}
