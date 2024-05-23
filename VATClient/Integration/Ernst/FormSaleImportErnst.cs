using Newtonsoft.Json;
using SymphonySofttech.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATViewModel.Integration.JsonModels;

namespace VATClient.Integration.Ernst
{
    public partial class FormSaleImportErnst : Form
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

        private bool isProcessRunning;

        string rootPath = @"";
        string processPath = @"";
        string iasPath = @"";

        string ibsPath1 = @"";
        string iasPath1 = @"";

        #endregion

        public FormSaleImportErnst()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType)
        {
            FormSaleImportErnst form = new FormSaleImportErnst();
            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        private void FormSaleImportErnst_Load(object sender, EventArgs e)
        {
            ////tableLoad();
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


        private void btnBulk_Click(object sender, EventArgs e)
        {

            try
            {

                rootPath = @"Z:\SDC_QA2\Output\out\BD\Request";

                processPath = @"D:\SymphonySofttech\SharePath";

                Directory.CreateDirectory(processPath + "\\Hold");
                Directory.CreateDirectory(processPath + "\\Debug");
                Directory.CreateDirectory(processPath + "\\Posted");
                Directory.CreateDirectory(processPath + "\\Skipped");

                if (!timer1.Enabled)
                {
                    timer1.Start();
                }

                MessageBox.Show("Process Started");

            }
            catch (Exception exception)
            {
                FileLogger.Log("Start FIle Watcher", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!isProcessRunning)
                {

                    progressBar1.Visible = true;
                    isProcessRunning = true;

                    bgwErnst.RunWorkerAsync();

                }

            }
            catch (Exception exception)
            {
                FileLogger.Log("timer1_Tick", "timer1_Tick", exception.Message + "\n" + exception.StackTrace);
            }

        }

        private void bgwErnst_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetFiles(rootPath);
            }
            catch (Exception exception)
            {
                FileLogger.Log("Start FIle Watcher", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);

            }
        }

        private void bgwErnst_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isProcessRunning = false;
            progressBar1.Visible = false;

        }

        public void GetFiles(string directoryPath)
        {
            string fileName = "first";

            string filePath = "";
            filePath = directoryPath;//+ "\\Work";

            string currentFilePath = "";

            try
            {

                string[] files = Directory.GetFiles(filePath);

                int loopCounter = 400;

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

                        using (StreamReader sr = new StreamReader(file))
                        {
                            string xml = sr.ReadToEnd();
                            sr.Close();
                            SaveXmlSale(xml, fileName);

                        }

                        string destinationFile = processPath + "\\Posted\\" + fileName;

                        MoveFile(file, destinationFile);

                    }
                    catch (Exception e)
                    {
                        if (e.Message.ToLower() == "debug")
                        {
                            MoveFile(currentFilePath, processPath + "\\Debug\\" + fileName);

                        }
                        else if (e.Message.ToLower() == "skip")
                        {
                            MoveFile(currentFilePath, processPath + "\\Skipped\\" + fileName);
                        }
                        else
                        {
                            MoveFile(currentFilePath, processPath + "\\Hold\\" + fileName);

                        }

                        FileLogger.Log("FormSaleImportErnst", "Getfiles", fileName + "\n" + e.Message + "\n" + e.StackTrace);
                    }
                }


            }
            catch (Exception e)
            {

                FileLogger.Log("FormSaleImportErnst", "Getfiles", fileName + "\n" + e.Message + "\n" + e.StackTrace);

            }

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

        private DataTable GetTableFromXML(string fileName)
        {
            #region Variables
            DataTable dt = new DataTable();

            #endregion

            if (string.IsNullOrEmpty(fileName))
            {
                return dt;
            }

            using (StreamReader sr = new StreamReader(fileName))
            {
                string xml = sr.ReadToEnd();
                DataSet dsResult = OrdinaryVATDesktop.GetDataSetFromXML(xml);

                if (dsResult.Tables.Count > 0)
                {
                    dt = dsResult.Tables[0];
                }

            }

            return dt;
        }

        private void TableValidation(DataTable salesData, string TType, string fileName)
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
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = TType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };
            var OtherRef = new DataColumn("OtherRef") { DefaultValue = fileName };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("OtherRef"))
            {
                salesData.Columns.Add(OtherRef);
            }
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

        private async void MoveFile(string sourceFile, string destinationFile)
        {
            try
            {
                using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
                {
                    using (FileStream destinationStream = File.Create(destinationFile))
                    {

                        await sourceStream.CopyToAsync(destinationStream);
                        sourceStream.Close();
                        File.Delete(sourceFile);

                    }
                }
            }
            catch (Exception e)
            {
                FileLogger.Log("DHL", "Getfiles", e.Message + "\n" + e.StackTrace);

            }
        }

        public string[] SaveXmlSale(string xml, string srcFileName)
        {
            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp();

            try
            {
                DataTable finalTable = new DataTable();
                SaleDAL saleDal = new SaleDAL();

                if (xml.Contains("<n0:CreditNote"))
                {
                    finalTable = GetErnstXMLData_Credit(xml, srcFileName);
                    TableValidation(finalTable, "Credit", srcFileName);
                }
                else
                {
                    finalTable = GetErnstSalesXMLData(xml, srcFileName);
                    TableValidation(finalTable, "ServiceNS", srcFileName);
                }

                if (finalTable.Rows.Count == 0)
                {
                    throw new Exception("skip");
                }

                //NewconnVM = Authenticate("PB23");


                string[] res = saleDal.SaveAndProcess(finalTable, () => { }, Program.BranchId, "", NewconnVM, null, null, null, Program.CurrentUserID);

                return res;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private DataTable GetErnstXMLData(string xml, string srcFileName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            ////DataTable finalTable = new DataTable();
            ////finalTable = PopulateMinimumColumn(finalTable);
            DataTable dt = new DataTable();

            ////DataTable tempTable = new DataTable();

            ////////tempTable = PopulateMinimumColumn(tempTable);

            try
            {

                string json = JsonConvert.SerializeObject(xmlDocument);

                JsonModels_Ernst ApiData = JsonConvert.DeserializeObject<JsonModels_Ernst>(json);



                #region Create DataTable

                dt.Clear();
                dt.Columns.Add("ID");
                ////dt.Columns.Add("InvoiceNo");
                ////dt.Columns.Add("Branch_Code");
                dt.Columns.Add("Customer_Code");
                dt.Columns.Add("Customer_Name");
                dt.Columns.Add("Invoice_Date_Time");
                dt.Columns.Add("Reference_No");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UOM");
                dt.Columns.Add("NBR_Price");
                dt.Columns.Add("SubTotal");
                dt.Columns.Add("VAT_Rate");
                dt.Columns.Add("VAT_Amount");
                dt.Columns.Add("Vehicle_No");
                dt.Columns.Add("VehicleType");
                dt.Columns.Add("CustomerGroup");
                dt.Columns.Add("Delivery_Address");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Sale_Type");
                //////dt.Columns.Add("TransactionType");
                dt.Columns.Add("Discount_Amount");
                dt.Columns.Add("Promotional_Quantity");

                dt.Columns.Add("SD_Rate");
                dt.Columns.Add("SDAmount");
                dt.Columns.Add("Is_Print");
                dt.Columns.Add("Tender_Id");
                dt.Columns.Add("Post");
                dt.Columns.Add("LC_Number");
                dt.Columns.Add("Currency_Code");
                dt.Columns.Add("CommentsD");
                dt.Columns.Add("Non_Stock");
                dt.Columns.Add("Trading_MarkUp");
                dt.Columns.Add("Type");
                dt.Columns.Add("VAT_Name");

                dt.Columns.Add("Previous_Invoice_No");
                dt.Columns.Add("PreviousInvoiceDateTime");
                dt.Columns.Add("PreviousNBRPrice");
                dt.Columns.Add("PreviousQuantity");
                dt.Columns.Add("PreviousUOM");
                dt.Columns.Add("PreviousSubTotal");
                dt.Columns.Add("PreviousVATAmount");
                dt.Columns.Add("PreviousVATRate");
                dt.Columns.Add("PreviousSD");
                dt.Columns.Add("PreviousSDAmount");
                dt.Columns.Add("ReasonOfReturn");
                ////dt.Columns.Add("TransactionType");
                ////dt.Columns.Add("CompanyCode");

                #endregion

                #region Sales

                string CurrentTime = DateTime.Now.ToString("HH:mm:ss");

                foreach (var items in ApiData.N0Invoice.N1InvoiceLine)
                {
                    string ID = ApiData.N0Invoice.N2ID;
                    string Branch_Code = "001";
                    string Invoice_Date_Time = Convert.ToDateTime(ApiData.N0Invoice.N2IssueDate).ToString("yyyy-MM-dd " + CurrentTime);
                    string DeliveryDateTime = Invoice_Date_Time;
                    string Customer_Code = ApiData.N0Invoice.N1AccountingCustomerParty.N2SupplierAssignedAccountID;
                    string Customer_Name = ApiData.N0Invoice.N1AccountingCustomerParty.N1Party.N1PartyName.N2Name;
                    if (string.IsNullOrEmpty(Customer_Name))
                    {
                        Customer_Name = "";
                    }
                    string Comments = "-";
                    string StreetName = ApiData.N0Invoice.N1AccountingCustomerParty.N1Party.N1PostalAddress.N2StreetName;
                    string CityName = ApiData.N0Invoice.N1AccountingCustomerParty.N1Party.N1PostalAddress.N2CityName;
                    string PostalZone = ApiData.N0Invoice.N1AccountingCustomerParty.N1Party.N1PostalAddress.N2PostalZone;

                    string Delivery_Address = StreetName + "," + CityName + "," + PostalZone;

                    DataRow dr = dt.NewRow();
                    dr["ID"] = ID;
                    ////dr["Branch_Code"] = Branch_Code;
                    dr["Customer_Code"] = Customer_Code;
                    dr["Customer_Name"] = Customer_Name;
                    dr["Invoice_Date_Time"] = Invoice_Date_Time;
                    dr["Reference_No"] = ID;
                    dr["Delivery_Address"] = Delivery_Address;
                    dr["Comments"] = Comments;

                    dr["Item_Code"] = "0";
                    dr["Item_Name"] = items.N1Item.N2Description;
                    dr["UOM"] = "Unit";
                    dr["Quantity"] = Convert.ToDecimal(items.N1Price.N2BaseQuantity.Text);
                    dr["NBR_Price"] = Convert.ToDecimal(items.N1Price.N2PriceAmount.Text);
                    dr["VAT_Amount"] = Convert.ToDecimal(items.N1TaxTotal.N1TaxSubtotal.N2TaxAmount.Text);
                    dr["SD_Rate"] = "0";
                    dr["SDAmount"] = 0;

                    dr["Currency_Code"] = ApiData.N0Invoice.N2DocumentCurrencyCode;
                    dr["SubTotal"] = 0;
                    dr["Discount_Amount"] = 0;

                    ////dr["TransactionType"] = "Other";
                    //dr["CompanyCode"] = code;

                    decimal VATRate = 0;
                    VATRate = Convert.ToDecimal(items.N1TaxTotal.N1TaxSubtotal.N1TaxCategory.N2Percent);

                    dr["VAT_Rate"] = VATRate;

                    if (VATRate == 15)
                    {
                        dr["Type"] = "VAT";
                    }
                    else if (VATRate == 0)
                    {
                        dr["Type"] = "NoNVAT";
                    }
                    else
                    {
                        dr["Type"] = "OtherRate";
                    }

                    dr["Promotional_Quantity"] = "0";
                    dr["Sale_Type"] = "New";
                    dr["Vehicle_No"] = "NA";
                    dr["VehicleType"] = "NA";
                    dr["SD_Rate"] = "0";
                    dr["Is_Print"] = "N";
                    dr["Tender_Id"] = "0";
                    dr["Post"] = "N";
                    dr["LC_Number"] = "NA";
                    dr["CommentsD"] = "NA";
                    dr["Non_Stock"] = "N";
                    dr["Trading_MarkUp"] = "0";
                    dr["VAT_Name"] = "VAT 4.3";
                    dr["Previous_Invoice_No"] = "0";
                    dr["PreviousNBRPrice"] = "0";
                    dr["PreviousQuantity"] = "0";
                    dr["PreviousSubTotal"] = "0";
                    dr["PreviousVATAmount"] = "0";
                    dr["PreviousVATRate"] = "0";
                    dr["PreviousSD"] = "0";
                    dr["PreviousSDAmount"] = "0";
                    dr["ReasonOfReturn"] = "NA";

                    dt.Rows.Add(dr);

                }
                #endregion


                #region Comment

                ////XmlNodeList invoices = xmlDocument.GetElementsByTagName("n0:Invoice");

                ////ProductDAL productDal = new ProductDAL();

                ////foreach (XmlNode invoice in invoices)
                ////{
                ////    //////string ID = invoice.Attributes["n2:ID"].InnerText;

                ////    XmlNodeList IDs = invoice.SelectNodes("n2:ID");

                ////    string ID = invoice.SelectSingleNode("n2:ID").InnerText;

                ////    string refNo = ID;
                ////    XmlNode customer = invoice.SelectSingleNode("ADDR[@TyCd='IV']");
                ////    string customerName = customer["Name"].InnerText;
                ////    string delivaeryAdd = "";
                ////    string bin = "-";
                ////    string saleType = "NEW";
                ////    string transactionType = "Other";

                ////    string vatType = "VAT";
                ////    string vatRate = "15";
                ////    string cnDnref = "";
                ////    string branch_Code = "";
                ////    string revenueIdentifier = "";
                ////    List<string> orginInvoices = new List<string>();
                ////    decimal grandTotal = 0;

                ////    string productRefNo = "";

                ////    #region Customer Info

                ////    XmlNodeList streets = customer.SelectNodes("Street");

                ////    foreach (XmlNode st in streets)
                ////    {
                ////        if (!string.IsNullOrEmpty(st.InnerText))
                ////        {
                ////            delivaeryAdd += st.InnerText + "\n";
                ////        }
                ////    }

                ////    if (customer.SelectSingleNode("PostCd") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("PostCd").InnerText))
                ////    {
                ////        delivaeryAdd += customer.SelectSingleNode("PostCd").InnerText + " ";
                ////    }

                ////    if (customer.SelectSingleNode("CtyNm") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("CtyNm").InnerText))
                ////    {
                ////        delivaeryAdd += customer.SelectSingleNode("CtyNm").InnerText;
                ////    }

                ////    if (customer.SelectSingleNode("CtryCd") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("CtryCd").InnerText))
                ////    {
                ////        RegionInfo info = new RegionInfo(customer.SelectSingleNode("CtryCd").InnerText);

                ////        delivaeryAdd += " " + info.DisplayName.ToUpper();
                ////    }

                ////    if (customer.SelectSingleNode("RegNum") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("RegNum").InnerText))
                ////    {
                ////        bin = customer.SelectSingleNode("RegNum").InnerText;
                ////    }

                ////    #endregion

                ////    string accountCode = invoice.SelectSingleNode("Acc[@TyCd='PAYER']").InnerText;
                ////    string invoiceDateTime = invoice.SelectSingleNode("Dtm[@TyCd='INV']").InnerText;

                ////    #region BranchCode

                ////    if (branch_Code.ToLower() == "hof")
                ////    {
                ////        revenueIdentifier = "A";
                ////    }
                ////    else if (branch_Code.ToLower() == "tej")
                ////    {
                ////        revenueIdentifier = "P";
                ////    }
                ////    else
                ////    {
                ////        revenueIdentifier = "R";
                ////    }

                ////    #endregion


                ////    //XmlNodeList test = invoice.SelectNodes("UNIT[@ProdSrvCd!='Z']");


                ////    XmlNode node = invoice.SelectSingleNode("//SrvA[@TyCd='ORIGIN']");

                ////    string billsyst = invoice.SelectSingleNode("REFS").SelectSingleNode("REF[@TyCd='BILLSYST']").InnerText;

                ////    #region SaleType

                ////    string type = invoice.SelectSingleNode("REFS").SelectSingleNode("REF[@TyCd='INVTYPE']").InnerText;

                ////    if (type == "N")
                ////    {
                ////        saleType = "credit";
                ////        transactionType = "credit";

                ////    }
                ////    else if (type == "D")
                ////    {
                ////        saleType = "debit";
                ////        transactionType = "debit";
                ////    }

                ////    #endregion

                ////    #region Invoice Type

                ////    string accCsh = "ACC";
                ////    if (!OrdinaryVATDesktop.IsNumber(accountCode))
                ////    {
                ////        accCsh = "CSH";
                ////    }

                ////    #endregion


                ////    #region File Name Generate

                ////    string fileName = "^" + accountCode + "^" + ID + "^" + invoiceDateTime.Replace("-", "") + "^" +
                ////                      node.SelectSingleNode("SrvACd").InnerText + "^^" + accCsh + "^^^^^" +
                ////                      invoice.SelectSingleNode("REFS").SelectSingleNode("REF[@TyCd='BILLCTRY']")
                ////                          .InnerText + "^^" +
                ////                      billsyst.Substring(0, billsyst.Length - 1) +
                ////                      "^^BIV^OB^EBILL^COPY_VTI_DAC_CO_SPN_@gendate_@gentime";

                ////    #endregion

                ////    invoiceDateTime += " 16:00";

                ////    #region line iteams
                ////    XmlNodeList lineItems = invoice.SelectNodes("UNIT[@ProdSrvCd!='Z']");

                ////    XmlNodeList lineItemsZ = invoice.SelectNodes("UNIT[@ProdSrvCd='Z']");


                ////    #region LineItemsRegular

                ////    if (lineItems != null)
                ////    {
                ////        string originInvoice = "";
                ////        foreach (XmlNode item in lineItems)
                ////        {
                ////            finalTable.Rows.Add(finalTable.NewRow());
                ////            tempTable.Rows.Add(tempTable.NewRow());

                ////            int count = finalTable.Rows.Count - 1;
                ////            originInvoice = "";

                ////            #region Master

                ////            if (string.IsNullOrEmpty(branch_Code))
                ////            {
                ////                throw new Exception("Branch Code Not Found");
                ////            }

                ////            finalTable.Rows[count]["ID"] = ID;
                ////            finalTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                ////            finalTable.Rows[count]["Customer_Name"] = customerName;
                ////            finalTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                ////            finalTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                ////            finalTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                ////            finalTable.Rows[count]["SD_Rate"] = "0";

                ////            finalTable.Rows[count]["Non_Stock"] = "N";
                ////            finalTable.Rows[count]["Trading_MarkUp"] = "0";
                ////            finalTable.Rows[count]["Currency_Code"] = "BDT";
                ////            finalTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                ////            finalTable.Rows[count]["Post"] = "Y";
                ////            finalTable.Rows[count]["DataSource"] = "IBS";
                ////            finalTable.Rows[count]["Branch_Code"] = branch_Code;

                ////            string itemCode = item.Attributes["ProdSrvCd"].InnerText;
                ////            string comments = item.Attributes["ItemNo"].InnerText;

                ////            #endregion

                ////            XmlNode itemDetails = item.SelectSingleNode("CHGLN[@LineNo='1']");
                ////            XmlNode reasonOfReturnNode = item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("FTXS");
                ////            XmlNode DtmDate = item.SelectSingleNode("Dtm[@TyCd='UNIT']");
                ////            string returnDate = DtmDate != null ? DtmDate.InnerText : "1900-01-01";
                ////            string reasonOfReturn = "";

                ////            if (reasonOfReturnNode != null)
                ////            {
                ////                reasonOfReturn = reasonOfReturnNode.SelectSingleNode("FTX[@TyCd='CLD']").InnerText;
                ////            }


                ////            string itemName = itemDetails.Attributes["Dsc"].InnerText;

                ////            #region UOM and Dollar Value Ref

                ////            List<ProductVM> products = productDal.SelectAll("0", new[] { "pr.ProductName" }, new[] { itemName }, null, null, null, connVM);

                ////            if (products != null && products.Any())
                ////            {
                ////                finalTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                ////                tempTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                ////            }
                ////            else
                ////            {
                ////                finalTable.Rows[count]["UOM"] = "Shipment";
                ////                tempTable.Rows[count]["UOM"] = "Shipment";
                ////            }


                ////            string otherRef = "<otherRef><dollarRate>" + itemDetails.SelectSingleNode("ExRate").InnerText +
                ////                              "</dollarRate>"
                ////                              + "<dollarValue>" + itemDetails.SelectSingleNode("Val[@TyCd='INV']")
                ////                                  .InnerText + "</dollarValue><fileName>" + srcFileName + "</fileName></otherRef>";

                ////            #endregion

                ////            #region Price Calculation

                ////            string qunatity = "1";
                ////            //item.SelectSingleNode("MEAS[@TyCd='EA']") == null
                ////            //? "1"
                ////            //: item.SelectSingleNode("MEAS[@TyCd='EA']").InnerText;

                ////            string weight = item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']") == null
                ////                ? "0"
                ////                : item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']").InnerText;


                ////            XmlNodeList prices = item.SelectNodes("CHGLN");

                ////            decimal onlyPrice = 0;
                ////            decimal charges = 0;

                ////            decimal vatAmount = 0;
                ////            decimal lineTotal = 0;
                ////            decimal TemplineTotal = 0;
                ////            decimal tempVatAmount = 0;

                ////            foreach (XmlNode price in prices)
                ////            {

                ////                XmlNode tax = price.SelectSingleNode("TAX");

                ////                if ((price.SelectSingleNode("TAX[@TxCd='A']") == null && price.SelectSingleNode("TAX[@TxCd='B']") == null) && tax != null) continue;


                ////                string priceAndChrgs = "0";

                ////                priceAndChrgs = tax != null
                ////                    ? tax.SelectSingleNode("AmtSubj[@TyCd='CHG']").InnerText
                ////                    : price.SelectSingleNode("Val[@TyCd='CHG']").InnerText;


                ////                if (price.Attributes["LineNo"].InnerText != "1")
                ////                {
                ////                    charges += Convert.ToDecimal(priceAndChrgs);
                ////                }
                ////                else
                ////                {
                ////                    onlyPrice += Convert.ToDecimal(priceAndChrgs);

                ////                    if (price.Attributes != null && price.Attributes["Units"] != null && price.Attributes["Units"].InnerText.Trim().StartsWith("-"))
                ////                    {
                ////                        productRefNo = price.Attributes["Dsc"].InnerText;
                ////                    }
                ////                }

                ////                if (tax != null)
                ////                {
                ////                    vatAmount += Convert.ToDecimal(tax.SelectSingleNode("Val[@TyCd='CHG']").InnerText);
                ////                }


                ////                XmlNode lineTotals = price.SelectSingleNode("TOTS");

                ////                XmlNode lineTotalxml = lineTotals.SelectSingleNode("TOT[@TyCd='LINETOT_LCL']");

                ////                lineTotal += Convert.ToDecimal(lineTotalxml.InnerText);

                ////            }


                ////            decimal
                ////                totalPrice =
                ////                    onlyPrice +
                ////                    charges; //item.SelectSingleNode("TOTS").SelectSingleNode("TOT[@TyCd='UNIT_TOT_LCL']").InnerText;

                ////            if (totalPrice == 0)
                ////            {
                ////                //throw new ArgumentException("Price cant be 0");
                ////                finalTable.Rows.RemoveAt(count);
                ////                continue;
                ////            }
                ////            decimal originnalPrice = totalPrice / Convert.ToDecimal(qunatity);
                ////            decimal TempOriginnalPrice = totalPrice / Convert.ToDecimal(qunatity);

                ////            grandTotal += (totalPrice + vatAmount);
                ////            tempVatAmount = vatAmount;


                ////            if (originnalPrice < 0)
                ////            {
                ////                originnalPrice = originnalPrice * -1;
                ////            }

                ////            if (vatAmount < 0)
                ////            {
                ////                vatAmount = vatAmount * -1;
                ////            }

                ////            TemplineTotal = lineTotal;
                ////            if (lineTotal < 0)
                ////                lineTotal *= -1;

                ////            #endregion


                ////            #region VAT Type

                ////            bool itemTax =
                ////                item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("TAX") == null ||
                ////                item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("TAX[@TxCd='B']") != null;

                ////            if (itemTax)
                ////            {
                ////                vatType = "NonVAT";
                ////                vatRate = "0";
                ////            }
                ////            else
                ////            {
                ////                vatType = "VAT";
                ////                vatRate = "15";
                ////            }

                ////            if (vatAmount == 0)
                ////            {
                ////                vatType = "NonVAT";
                ////                vatRate = "0";
                ////            }

                ////            #endregion


                ////            //decimal vatRate = 15;
                ////            //decimal priceWithOutVat = (Convert.ToDecimal(totalPrice) * (100 / (100 + vatRate)));

                ////            #region CN DN Ref

                ////            XmlNode lineItemRefs = item.SelectSingleNode("REFS");

                ////            if (lineItemRefs != null)
                ////            {
                ////                XmlNodeList orginInvoice = lineItemRefs.SelectNodes("REF[@TyCd='ORIGINVNO']");

                ////                if (orginInvoice != null)
                ////                {
                ////                    foreach (XmlNode xmlNode in orginInvoice)
                ////                    {
                ////                        if (!orginInvoices.Contains(xmlNode.InnerText))
                ////                        {
                ////                            orginInvoices.Add(xmlNode.InnerText);
                ////                        }

                ////                        originInvoice = xmlNode.InnerText;

                ////                    }
                ////                }

                ////            }

                ////            #endregion

                ////            #region Details

                ////            finalTable.Rows[count]["Item_Name"] = itemName;
                ////            finalTable.Rows[count]["Item_Code"] = "-";//itemCode;

                ////            finalTable.Rows[count]["Quantity"] = qunatity;

                ////            finalTable.Rows[count]["ExtraCharge"] = charges;
                ////            finalTable.Rows[count]["VAT_Amount"] = vatAmount;
                ////            finalTable.Rows[count]["Sale_Type"] = saleType;
                ////            finalTable.Rows[count]["TransactionType"] = transactionType;
                ////            finalTable.Rows[count]["VAT_Rate"] = vatRate;
                ////            finalTable.Rows[count]["Type"] = vatType;


                ////            finalTable.Rows[count]["LineTotal"] = lineTotal;


                ////            finalTable.Rows[count]["NBR_Price"] =
                ////                originnalPrice;

                ////            finalTable.Rows[count]["Weight"] =
                ////                weight;

                ////            finalTable.Rows[count]["CommentsD"] =
                ////                comments;

                ////            finalTable.Rows[count]["FileName"] =
                ////                fileName;

                ////            finalTable.Rows[count]["OtherRef"] =
                ////                otherRef;

                ////            finalTable.Rows[count]["CustomerBIN"] =
                ////                bin;

                ////            finalTable.Rows[count]["ReasonOfReturn"] =
                ////                reasonOfReturn;

                ////            finalTable.Rows[count]["Previous_Invoice_No"] =
                ////                originInvoice;

                ////            finalTable.Rows[count]["PreviousInvoiceDateTime"] =
                ////                returnDate;

                ////            #endregion

                ////            #region Temp Details

                ////            tempTable.Rows[count]["Item_Name"] = itemName;
                ////            tempTable.Rows[count]["Item_Code"] = "-";//itemCode;

                ////            tempTable.Rows[count]["Quantity"] = qunatity;

                ////            tempTable.Rows[count]["ExtraCharge"] = charges;
                ////            tempTable.Rows[count]["VAT_Amount"] = tempVatAmount;
                ////            tempTable.Rows[count]["Sale_Type"] = saleType;
                ////            tempTable.Rows[count]["TransactionType"] = transactionType;
                ////            tempTable.Rows[count]["VAT_Rate"] = vatRate;
                ////            tempTable.Rows[count]["Type"] = vatType;
                ////            tempTable.Rows[count]["SubTotal"] = totalPrice;

                ////            tempTable.Rows[count]["LineTotal"] = TemplineTotal;


                ////            tempTable.Rows[count]["NBR_Price"] =
                ////                TempOriginnalPrice;

                ////            tempTable.Rows[count]["Weight"] =
                ////                weight;

                ////            tempTable.Rows[count]["CommentsD"] =
                ////                comments;

                ////            tempTable.Rows[count]["FileName"] =
                ////                fileName;

                ////            tempTable.Rows[count]["OtherRef"] =
                ////                otherRef;

                ////            tempTable.Rows[count]["CustomerBIN"] =
                ////                bin;

                ////            tempTable.Rows[count]["ReasonOfReturn"] =
                ////                reasonOfReturn;


                ////            tempTable.Rows[count]["Previous_Invoice_No"] =
                ////                originInvoice;

                ////            tempTable.Rows[count]["PreviousInvoiceDateTime"] =
                ////                returnDate;

                ////            #endregion

                ////        }
                ////    }

                ////    #endregion

                ////    #region LineItemsZ

                ////    if (lineItemsZ != null)
                ////    {
                ////        string originInvoice = "";
                ////        foreach (XmlNode item in lineItemsZ)
                ////        {


                ////            XmlNode reasonOfReturnNode = item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("FTXS");
                ////            string reasonOfReturn = "";

                ////            if (reasonOfReturnNode != null)
                ////            {
                ////                reasonOfReturn = reasonOfReturnNode.SelectSingleNode("FTX[@TyCd='CLD']").InnerText;
                ////            }

                ////            XmlNode DtmDate = item.SelectSingleNode("Dtm[@TyCd='UNIT']");
                ////            string returnDate = DtmDate != null ? DtmDate.InnerText : "1900-01-01";

                ////            XmlNode itemDetails = item.SelectSingleNode("CHGLN[@LineNo='1']");


                ////            string weight = item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']") == null
                ////                ? "0"
                ////                : item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']").InnerText;

                ////            #region UOM and Dollar Value Ref

                ////            //List<ProductVM> products = productDal.SelectAll("0", new[] { "pr.ProductName" }, new[] { itemName });

                ////            //if (products != null && products.Any())
                ////            //{
                ////            //    finalTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                ////            //}
                ////            //else
                ////            //{
                ////            //    finalTable.Rows[count]["UOM"] = "Shipment";
                ////            //}



                ////            string otherRef = "<otherRef><dollarRate>" +
                ////                              itemDetails.SelectSingleNode("ExRate").InnerText +
                ////                              "</dollarRate>"
                ////                              + "<dollarValue>" + itemDetails.SelectSingleNode("Val[@TyCd='INV']")
                ////                                  .InnerText + "</dollarValue><fileName>" + srcFileName +
                ////                              "</fileName></otherRef>";

                ////            #endregion


                ////            XmlNodeList prices = item.SelectNodes("CHGLN");


                ////            foreach (XmlNode price in prices)
                ////            {
                ////                ////var rows = chargeCodes.Select("ChargeCode = '" +
                ////                ////                   price.Attributes["ChgCd"].InnerText.ToUpper() + "'");

                ////                // !price.Attributes["ChgCd"].InnerText.ToLower().StartsWith("w")
                ////                ////if (rows.Length > 0)
                ////                ////{
                ////                ////    if (price.SelectSingleNode("TAX[@TxCd='A']") == null && price.SelectSingleNode("TAX[@TxCd='B']") == null)
                ////                ////        continue;
                ////                ////}


                ////                finalTable.Rows.Add(finalTable.NewRow());
                ////                tempTable.Rows.Add(tempTable.NewRow());

                ////                int count = finalTable.Rows.Count - 1;

                ////                #region Master

                ////                if (string.IsNullOrEmpty(branch_Code))
                ////                {
                ////                    throw new Exception("Branch Code Not Found");
                ////                }

                ////                finalTable.Rows[count]["ID"] = ID;
                ////                finalTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                ////                finalTable.Rows[count]["Customer_Name"] = customerName;
                ////                finalTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                ////                finalTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                ////                finalTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                ////                finalTable.Rows[count]["SD_Rate"] = "0";

                ////                finalTable.Rows[count]["Non_Stock"] = "N";
                ////                finalTable.Rows[count]["Trading_MarkUp"] = "0";
                ////                finalTable.Rows[count]["Currency_Code"] = "BDT";
                ////                finalTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                ////                finalTable.Rows[count]["Post"] = "Y";
                ////                finalTable.Rows[count]["Branch_Code"] = branch_Code;

                ////                finalTable.Rows[count]["DataSource"] = "IBS";
                ////                finalTable.Rows[count]["UOM"] = "Shipment";


                ////                #region Temp table data assign

                ////                tempTable.Rows[count]["ID"] = ID;
                ////                tempTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                ////                tempTable.Rows[count]["Customer_Name"] = customerName;
                ////                tempTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                ////                tempTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                ////                tempTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                ////                tempTable.Rows[count]["SD_Rate"] = "0";

                ////                tempTable.Rows[count]["Non_Stock"] = "N";
                ////                tempTable.Rows[count]["Trading_MarkUp"] = "0";
                ////                tempTable.Rows[count]["Currency_Code"] = "BDT";
                ////                tempTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                ////                tempTable.Rows[count]["Post"] = "Y";
                ////                tempTable.Rows[count]["Branch_Code"] = branch_Code;

                ////                tempTable.Rows[count]["DataSource"] = "IBS";
                ////                tempTable.Rows[count]["UOM"] = "Shipment";

                ////                #endregion


                ////                string itemCode = item.Attributes["ProdSrvCd"].InnerText;
                ////                string comments = item.Attributes["ItemNo"].InnerText;

                ////                #endregion


                ////                decimal onlyPrice = 0;
                ////                decimal charges = 0;
                ////                string qunatity = "1";

                ////                decimal vatAmount = 0;
                ////                decimal tempVatAmount = 0;

                ////                XmlNode tax = price.SelectSingleNode("TAX");

                ////                string priceAndChrgs = "0";


                ////                string itemName = price.Attributes["Dsc"].InnerText;

                ////                priceAndChrgs = tax != null
                ////                    ? tax.SelectSingleNode("AmtSubj[@TyCd='CHG']").InnerText
                ////                    : price.SelectSingleNode("Val[@TyCd='CHG']").InnerText;


                ////                onlyPrice = Convert.ToDecimal(priceAndChrgs);

                ////                if (tax != null)
                ////                {
                ////                    vatAmount += Convert.ToDecimal(tax.SelectSingleNode("Val[@TyCd='CHG']").InnerText);
                ////                }

                ////                XmlNode lineTotals = price.SelectSingleNode("TOTS");

                ////                XmlNode lineTotalxml = lineTotals.SelectSingleNode("TOT[@TyCd='LINETOT_LCL']");

                ////                decimal lineTotal = Convert.ToDecimal(lineTotalxml.InnerText);
                ////                decimal tempLineTotal = Convert.ToDecimal(lineTotalxml.InnerText);

                ////                decimal
                ////                    totalPrice =
                ////                        onlyPrice +
                ////                        charges; //item.SelectSingleNode("TOTS").SelectSingleNode("TOT[@TyCd='UNIT_TOT_LCL']").InnerText;

                ////                if (totalPrice == 0)
                ////                {
                ////                    finalTable.Rows.RemoveAt(count);
                ////                    continue;
                ////                }
                ////                decimal originnalPrice = totalPrice / Convert.ToDecimal(qunatity);
                ////                decimal TemporiginnalPrice = totalPrice / Convert.ToDecimal(qunatity);

                ////                grandTotal += (totalPrice + vatAmount);
                ////                tempVatAmount = vatAmount;

                ////                if (originnalPrice < 0)
                ////                    originnalPrice = originnalPrice * -1;

                ////                if (vatAmount < 0)
                ////                    vatAmount = vatAmount * -1;

                ////                if (lineTotal < 0)
                ////                    lineTotal *= -1;


                ////                if (price.Attributes != null && price.Attributes["Units"] != null && price.Attributes["Units"].InnerText.Trim().StartsWith("-"))
                ////                {
                ////                    productRefNo = price.Attributes["Dsc"].InnerText;
                ////                }

                ////                #region VAT Type

                ////                bool itemTax = price.SelectSingleNode("TAX") == null ||
                ////                               price.SelectSingleNode("TAX[@TxCd='B']") != null;

                ////                if (itemTax)
                ////                {
                ////                    vatType = "NonVAT";
                ////                    vatRate = "0";
                ////                }
                ////                else
                ////                {
                ////                    vatType = "VAT";
                ////                    vatRate = "15";
                ////                }

                ////                if (vatAmount == 0)
                ////                {
                ////                    vatType = "NonVAT";
                ////                    vatRate = "0";
                ////                }

                ////                #endregion

                ////                #region CN DN Ref

                ////                XmlNode lineItemRefs = item.SelectSingleNode("REFS");

                ////                if (lineItemRefs != null)
                ////                {
                ////                    XmlNodeList orginInvoice = lineItemRefs.SelectNodes("REF[@TyCd='ORIGINVNO']");

                ////                    if (orginInvoice != null)
                ////                    {
                ////                        foreach (XmlNode xmlNode in orginInvoice)
                ////                        {
                ////                            if (!orginInvoices.Contains(xmlNode.InnerText))
                ////                            {
                ////                                orginInvoices.Add(xmlNode.InnerText);
                ////                            }

                ////                            originInvoice = xmlNode.InnerText;

                ////                        }
                ////                    }

                ////                }

                ////                #endregion

                ////                #region Details

                ////                finalTable.Rows[count]["Item_Name"] = itemName;
                ////                finalTable.Rows[count]["Item_Code"] = "-";//itemCode;

                ////                finalTable.Rows[count]["Quantity"] = qunatity;

                ////                finalTable.Rows[count]["ExtraCharge"] = charges;
                ////                finalTable.Rows[count]["VAT_Amount"] = vatAmount;
                ////                finalTable.Rows[count]["SubTotal"] = totalPrice;

                ////                finalTable.Rows[count]["Sale_Type"] = saleType;
                ////                finalTable.Rows[count]["TransactionType"] = transactionType;
                ////                finalTable.Rows[count]["VAT_Rate"] = vatRate;
                ////                finalTable.Rows[count]["Type"] = vatType;

                ////                finalTable.Rows[count]["LineTotal"] = lineTotal;

                ////                finalTable.Rows[count]["NBR_Price"] =
                ////                    originnalPrice;

                ////                finalTable.Rows[count]["Weight"] =
                ////                    weight;

                ////                finalTable.Rows[count]["CommentsD"] =
                ////                    comments;

                ////                finalTable.Rows[count]["FileName"] =
                ////                    fileName;

                ////                finalTable.Rows[count]["OtherRef"] =
                ////                    otherRef;

                ////                finalTable.Rows[count]["CustomerBIN"] =
                ////                    bin;

                ////                finalTable.Rows[count]["ReasonOfReturn"] =
                ////                    reasonOfReturn;

                ////                finalTable.Rows[count]["Previous_Invoice_No"] =
                ////                    originInvoice;
                ////                finalTable.Rows[count]["PreviousInvoiceDateTime"] =
                ////                    returnDate;

                ////                #endregion

                ////                #region Temp table

                ////                tempTable.Rows[count]["Item_Name"] = itemName;
                ////                tempTable.Rows[count]["Item_Code"] = "-";//itemCode;

                ////                tempTable.Rows[count]["Quantity"] = qunatity;

                ////                tempTable.Rows[count]["ExtraCharge"] = charges;
                ////                tempTable.Rows[count]["VAT_Amount"] = tempVatAmount;
                ////                tempTable.Rows[count]["SubTotal"] = totalPrice;

                ////                tempTable.Rows[count]["Sale_Type"] = saleType;
                ////                tempTable.Rows[count]["TransactionType"] = transactionType;
                ////                tempTable.Rows[count]["VAT_Rate"] = vatRate;
                ////                tempTable.Rows[count]["Type"] = vatType;

                ////                tempTable.Rows[count]["LineTotal"] = tempLineTotal;


                ////                tempTable.Rows[count]["NBR_Price"] =
                ////                    TemporiginnalPrice;

                ////                tempTable.Rows[count]["Weight"] =
                ////                    weight;

                ////                tempTable.Rows[count]["CommentsD"] =
                ////                    comments;

                ////                tempTable.Rows[count]["FileName"] =
                ////                    fileName;

                ////                tempTable.Rows[count]["OtherRef"] =
                ////                    otherRef;

                ////                tempTable.Rows[count]["CustomerBIN"] =
                ////                    bin;

                ////                tempTable.Rows[count]["ReasonOfReturn"] =
                ////                    reasonOfReturn;

                ////                tempTable.Rows[count]["Previous_Invoice_No"] =
                ////                    originInvoice;

                ////                tempTable.Rows[count]["PreviousInvoiceDateTime"] =
                ////                    returnDate;

                ////                #endregion

                ////            }

                ////        }
                ////    }

                ////    #endregion

                ////    #endregion

                ////    cnDnref = string.Join(",", orginInvoices);

                ////    #region Checking Transaction Type Logic Based on CashBD Account

                ////    if (accountCode.ToLower().StartsWith("cashbd"))
                ////    {


                ////        if (!string.IsNullOrEmpty(productRefNo))
                ////        {
                ////            DataRow[] rows = finalTable.Select("Item_Name= '" + productRefNo + "'");

                ////            if (rows.Length > 1)
                ////            {
                ////                CommonDAL commonDal = new CommonDAL();

                ////                ////tempTable = commonDal.GetCumulativeValue(tempTable, null, null, connVM);

                ////                tempTable.Columns.Add(new DataColumn()
                ////                {
                ////                    ColumnName = "OtherRef",
                ////                    DefaultValue = finalTable.Rows[0]["OtherRef"].ToString()
                ////                });

                ////                finalTable = tempTable.Copy();

                ////            }
                ////        }

                ////        if (grandTotal < 0)
                ////        {
                ////            saleType = "credit";
                ////            transactionType = "credit";

                ////            finalTable.Columns.Remove("TransactionType");
                ////            finalTable.Columns.Remove("Sale_Type");

                ////            finalTable.Columns.Add(new DataColumn() { ColumnName = "TransactionType", DefaultValue = transactionType });
                ////            finalTable.Columns.Add(new DataColumn() { ColumnName = "Sale_Type", DefaultValue = saleType });

                ////        }
                ////        else if (grandTotal > 0 && !string.IsNullOrEmpty(productRefNo))
                ////        {
                ////            saleType = "debit";
                ////            transactionType = "debit";

                ////            finalTable.Columns.Remove("TransactionType");
                ////            finalTable.Columns.Remove("Sale_Type");

                ////            finalTable.Columns.Add(new DataColumn() { ColumnName = "TransactionType", DefaultValue = transactionType });
                ////            finalTable.Columns.Add(new DataColumn() { ColumnName = "Sale_Type", DefaultValue = saleType });
                ////        }
                ////    }

                ////    #endregion

                ////    finalTable.Columns.Add(new DataColumn() { DefaultValue = cnDnref, ColumnName = "CNDNRef" });
                ////}

                #endregion

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

            return dt;
        }

        private DataTable GetErnstSalesXMLData(string xml, string srcFileName)
        {

            #region xml Replace

            string rXML = xml.Replace(@"xmlns:n0=""urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"" xmlns:prx=""urn:sap.com:proxy:PE1:/1SAI/TAS8AE72A243AE3DD0533E4:750"" xmlns:n1=""urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"" xmlns:n2=""urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"" xmlns:n3=""urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2"" xmlns:n4=""urn:oasis:names:specification:ubl:schema:xsd:ExtensionAggregateComponents-2"" xmlns:n5=""urn:oasis:names:specification:ubl:schema:xsd:ExtensionBasicComponents-2""", "");

            rXML = rXML.Replace("n0:", "");
            rXML = rXML.Replace("n1:", "");
            rXML = rXML.Replace("n2:", "");
            rXML = rXML.Replace("n3:", "");
            rXML = rXML.Replace("n4:", "");
            rXML = rXML.Replace("n5:", "");

            #endregion

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(rXML);

            DataTable dt = new DataTable();

            try
            {
                XmlNodeList invoices = xmlDocument.GetElementsByTagName("Invoice");
                XmlNodeList invoicesLines = xmlDocument.GetElementsByTagName("InvoiceLine");

                #region Create DataTable

                dt.Clear();
                dt.Columns.Add("ID");
                ////dt.Columns.Add("InvoiceNo");
                ////dt.Columns.Add("Branch_Code");
                dt.Columns.Add("Customer_Code");
                dt.Columns.Add("Customer_Name");
                dt.Columns.Add("Invoice_Date_Time");
                dt.Columns.Add("Reference_No");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UOM");
                dt.Columns.Add("NBR_Price");
                dt.Columns.Add("SubTotal");
                dt.Columns.Add("VAT_Rate");
                dt.Columns.Add("VAT_Amount");
                dt.Columns.Add("Vehicle_No");
                dt.Columns.Add("VehicleType");
                dt.Columns.Add("CustomerGroup");
                dt.Columns.Add("Delivery_Address");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Sale_Type");
                //////dt.Columns.Add("TransactionType");
                dt.Columns.Add("Discount_Amount");
                dt.Columns.Add("Promotional_Quantity");

                dt.Columns.Add("SD_Rate");
                dt.Columns.Add("SDAmount");
                dt.Columns.Add("Is_Print");
                dt.Columns.Add("Tender_Id");
                dt.Columns.Add("Post");
                dt.Columns.Add("LC_Number");
                dt.Columns.Add("Currency_Code");
                dt.Columns.Add("CommentsD");
                dt.Columns.Add("Non_Stock");
                dt.Columns.Add("Trading_MarkUp");
                dt.Columns.Add("Type");
                dt.Columns.Add("VAT_Name");
                dt.Columns.Add("Previous_Invoice_No");
                dt.Columns.Add("PreviousInvoiceDateTime");
                dt.Columns.Add("PreviousNBRPrice");
                dt.Columns.Add("PreviousQuantity");
                dt.Columns.Add("PreviousUOM");
                dt.Columns.Add("PreviousSubTotal");
                dt.Columns.Add("PreviousVATAmount");
                dt.Columns.Add("PreviousVATRate");
                dt.Columns.Add("PreviousSD");
                dt.Columns.Add("PreviousSDAmount");
                dt.Columns.Add("ReasonOfReturn");
                ////dt.Columns.Add("TransactionType");
                dt.Columns.Add("CompanyCode");

                #endregion

                #region Sales

                string CurrentTime = DateTime.Now.ToString("HH:mm:ss");

                foreach (XmlNode invoice in invoices)
                {
                    foreach (XmlNode invoicesLine in invoicesLines)
                    {

                        XmlNode IDs = invoice.SelectSingleNode("ID");
                        XmlNode IssueDate = invoice.SelectSingleNode("IssueDate");
                        XmlNode ExchangeRate = invoice.SelectSingleNode("ExchangeRate");

                        XmlNode AccountingSupplierParty = invoice.SelectSingleNode("AccountingSupplierParty");
                        XmlNode cParty = AccountingSupplierParty.SelectSingleNode("Party");
                        XmlNode PartyIdentification = cParty.SelectSingleNode("PartyIdentification");


                        string ID = IDs.InnerText;
                        string CompanyCode = PartyIdentification["ID"].InnerText;

                        //string Branch_Code = "001";

                        string Invoice_Date_Time = Convert.ToDateTime(IssueDate.InnerText).ToString("yyyy-MM-dd " + CurrentTime);
                        string DeliveryDateTime = Invoice_Date_Time;

                        #region customer and Delivery Address

                        XmlNode customer = invoice.SelectSingleNode("AccountingCustomerParty");

                        XmlNode Party = customer.SelectSingleNode("Party");
                        XmlNode PartyName = Party.SelectSingleNode("PartyName");
                        XmlNode PostalAddress = Party.SelectSingleNode("PostalAddress");

                        string Customer_Code = customer["SupplierAssignedAccountID"].InnerText;
                        string Customer_Name = PartyName["Name"].InnerText;

                        if (string.IsNullOrEmpty(Customer_Name))
                        {
                            Customer_Name = "";
                        }

                        string StreetName = PostalAddress["StreetName"].InnerText;
                        string CityName = PostalAddress["CityName"].InnerText;
                        string PostalZone = PostalAddress["PostalZone"].InnerText;

                        string Delivery_Address = StreetName;

                        if (!string.IsNullOrWhiteSpace(CityName))
                        {
                            Delivery_Address = Delivery_Address + "," + CityName;
                        }
                        if (!string.IsNullOrWhiteSpace(PostalZone))
                        {
                            Delivery_Address = Delivery_Address + "," + PostalZone;
                        }

                        Delivery_Address = Delivery_Address.Replace(",,", ",");

                        #endregion

                        string Comments = "-";

                        DataRow dr = dt.NewRow();
                        dr["ID"] = ID;
                        ////dr["Branch_Code"] = Branch_Code;
                        dr["Customer_Code"] = Customer_Code;
                        dr["Customer_Name"] = Customer_Name;
                        dr["Invoice_Date_Time"] = Invoice_Date_Time;
                        dr["Reference_No"] = ID;
                        dr["Delivery_Address"] = Delivery_Address;
                        dr["Comments"] = Comments;

                        XmlNode Items = invoicesLine.SelectSingleNode("Item");
                        XmlNode Price = invoicesLine.SelectSingleNode("Price");
                        XmlNode TaxTotal = invoicesLine.SelectSingleNode("TaxTotal");
                        XmlNode TaxSubtotal = TaxTotal.SelectSingleNode("TaxSubtotal");
                        XmlNode TaxCategory = TaxSubtotal.SelectSingleNode("TaxCategory");


                        dr["Item_Code"] = "0";
                        dr["Item_Name"] = Items["Description"].InnerText;

                        XmlNode PackQuantity = Items.SelectSingleNode("PackQuantity ");
                        string UOM = PackQuantity.Attributes.GetNamedItem("unitCode").Value;

                        ////dr["UOM"] = "Unit";
                        dr["UOM"] = UOM;

                        dr["Quantity"] = Convert.ToDecimal(Price["BaseQuantity"].InnerText);
                        dr["NBR_Price"] = Convert.ToDecimal(Price["PriceAmount"].InnerText);

                        dr["VAT_Amount"] = Convert.ToDecimal(TaxSubtotal["TaxAmount"].InnerText);
                        dr["SD_Rate"] = "0";
                        dr["SDAmount"] = 0;

                        dr["Currency_Code"] = ExchangeRate["SourceCurrencyCode"].InnerText;
                        dr["SubTotal"] = 0;
                        dr["Discount_Amount"] = 0;

                        ////dr["TransactionType"] = "Other";
                        dr["CompanyCode"] = CompanyCode;

                        decimal VATRate = 0;
                        VATRate = Convert.ToDecimal(TaxCategory["Percent"].InnerText);

                        dr["VAT_Rate"] = VATRate;

                        if (VATRate == 15)
                        {
                            dr["Type"] = "VAT";
                        }
                        else if (VATRate == 0)
                        {
                            dr["Type"] = "NoNVAT";
                        }
                        else
                        {
                            dr["Type"] = "OtherRate";
                        }

                        dr["Promotional_Quantity"] = "0";
                        dr["Sale_Type"] = "New";
                        dr["Vehicle_No"] = "NA";
                        dr["VehicleType"] = "NA";
                        dr["SD_Rate"] = "0";
                        dr["Is_Print"] = "N";
                        dr["Tender_Id"] = "0";
                        dr["Post"] = "N";
                        dr["LC_Number"] = "NA";
                        dr["CommentsD"] = "NA";
                        dr["Non_Stock"] = "N";
                        dr["Trading_MarkUp"] = "0";
                        dr["VAT_Name"] = "VAT 4.3";

                        dr["Previous_Invoice_No"] = "0";
                        dr["PreviousNBRPrice"] = "0";
                        dr["PreviousQuantity"] = "0";
                        dr["PreviousSubTotal"] = "0";
                        dr["PreviousVATAmount"] = "0";
                        dr["PreviousVATRate"] = "0";
                        dr["PreviousSD"] = "0";
                        dr["PreviousSDAmount"] = "0";
                        dr["ReasonOfReturn"] = "NA";

                        dt.Rows.Add(dr);
                    }

                }
                
                #endregion

            }

            catch (ArgumentException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        private DataTable GetErnstXMLData_Credit(string xml, string srcFileName)
        {

            #region xml Replace

            string rXML = xml.Replace(@"xmlns:n0=""urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2"" xmlns:prx=""urn:sap.com:proxy:DE1:/1SAI/TASB056C066CC3F872C2539:750"" xmlns:n1=""urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"" xmlns:n2=""urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"" xmlns:n3=""urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2"" xmlns:n4=""urn:oasis:names:specification:ubl:schema:xsd:ExtensionAggregateComponents-2"" xmlns:n5=""urn:oasis:names:specification:ubl:schema:xsd:ExtensionBasicComponents-2""", "");

            rXML = rXML.Replace("n0:", "");
            rXML = rXML.Replace("n1:", "");
            rXML = rXML.Replace("n2:", "");
            rXML = rXML.Replace("n3:", "");
            rXML = rXML.Replace("n4:", "");
            rXML = rXML.Replace("n5:", "");

            #endregion

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(rXML);

            DataTable dt = new DataTable();

            try
            {

                XmlNodeList invoices = xmlDocument.GetElementsByTagName("CreditNote");
                XmlNodeList invoicesLines = xmlDocument.GetElementsByTagName("CreditNoteLine");

                #region Create DataTable

                dt.Clear();
                dt.Columns.Add("ID");
                ////dt.Columns.Add("InvoiceNo");
                ////dt.Columns.Add("Branch_Code");
                dt.Columns.Add("Customer_Code");
                dt.Columns.Add("Customer_Name");
                dt.Columns.Add("Invoice_Date_Time");
                dt.Columns.Add("Reference_No");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UOM");
                dt.Columns.Add("NBR_Price");
                dt.Columns.Add("SubTotal");
                dt.Columns.Add("VAT_Rate");
                dt.Columns.Add("VAT_Amount");
                dt.Columns.Add("Vehicle_No");
                dt.Columns.Add("VehicleType");
                dt.Columns.Add("CustomerGroup");
                dt.Columns.Add("Delivery_Address");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Sale_Type");
                //////dt.Columns.Add("TransactionType");
                dt.Columns.Add("Discount_Amount");
                dt.Columns.Add("Promotional_Quantity");

                dt.Columns.Add("SD_Rate");
                dt.Columns.Add("SDAmount");
                dt.Columns.Add("Is_Print");
                dt.Columns.Add("Tender_Id");
                dt.Columns.Add("Post");
                dt.Columns.Add("LC_Number");
                dt.Columns.Add("Currency_Code");
                dt.Columns.Add("CommentsD");
                dt.Columns.Add("Non_Stock");
                dt.Columns.Add("Trading_MarkUp");
                dt.Columns.Add("Type");
                dt.Columns.Add("VAT_Name");

                dt.Columns.Add("Previous_Invoice_No");
                dt.Columns.Add("PreviousInvoiceDateTime");
                dt.Columns.Add("PreviousNBRPrice");
                dt.Columns.Add("PreviousQuantity");
                dt.Columns.Add("PreviousUOM");
                dt.Columns.Add("PreviousSubTotal");
                dt.Columns.Add("PreviousVATAmount");
                dt.Columns.Add("PreviousVATRate");
                dt.Columns.Add("PreviousSD");
                dt.Columns.Add("PreviousSDAmount");
                dt.Columns.Add("ReasonOfReturn");
                ////dt.Columns.Add("TransactionType");
                dt.Columns.Add("CompanyCode");

                #endregion

                #region Cradit note

                string CurrentTime = DateTime.Now.ToString("HH:mm:ss");

                foreach (XmlNode invoice in invoices)
                {
                    foreach (XmlNode invoicesLine in invoicesLines)
                    {

                        XmlNode IDs = invoice.SelectSingleNode("ID");
                        XmlNode IssueDate = invoice.SelectSingleNode("IssueDate");
                        XmlNode ExchangeRate = invoice.SelectSingleNode("ExchangeRate");

                        XmlNode AccountingSupplierParty = invoice.SelectSingleNode("AccountingSupplierParty");
                        XmlNode cParty = AccountingSupplierParty.SelectSingleNode("Party");
                        XmlNode PartyIdentification = cParty.SelectSingleNode("PartyIdentification");

                        string ID = IDs.InnerText;
                        string CompanyCode = PartyIdentification["ID"].InnerText;

                        //string Branch_Code = "001";

                        string Invoice_Date_Time = Convert.ToDateTime(IssueDate.InnerText).ToString("yyyy-MM-dd " + CurrentTime);
                        string DeliveryDateTime = Invoice_Date_Time;

                        #region customer and Delivery Address

                        XmlNode customer = invoice.SelectSingleNode("AccountingCustomerParty");

                        XmlNode Party = customer.SelectSingleNode("Party");
                        XmlNode PartyName = Party.SelectSingleNode("PartyName");
                        XmlNode PostalAddress = Party.SelectSingleNode("PostalAddress");

                        string Customer_Code = customer["SupplierAssignedAccountID"].InnerText;
                        string Customer_Name = PartyName["Name"].InnerText;

                        if (string.IsNullOrEmpty(Customer_Name))
                        {
                            Customer_Name = "";
                        }

                        string StreetName = PostalAddress["StreetName"].InnerText;
                        string CityName = PostalAddress["CityName"].InnerText;
                        string PostalZone = PostalAddress["PostalZone"].InnerText;

                        string Delivery_Address = StreetName;

                        if (!string.IsNullOrWhiteSpace(CityName))
                        {
                            Delivery_Address = Delivery_Address + "," + CityName;
                        }
                        if (!string.IsNullOrWhiteSpace(PostalZone))
                        {
                            Delivery_Address = Delivery_Address + "," + PostalZone;
                        }
                        Delivery_Address = Delivery_Address.Replace(",,", ",");

                        #endregion

                        string Comments = "-";

                        DataRow dr = dt.NewRow();
                        dr["ID"] = ID;
                        ////dr["Branch_Code"] = Branch_Code;
                        dr["Customer_Code"] = Customer_Code;
                        dr["Customer_Name"] = Customer_Name;
                        dr["Invoice_Date_Time"] = Invoice_Date_Time;
                        dr["Reference_No"] = ID;
                        dr["Delivery_Address"] = Delivery_Address;
                        dr["Comments"] = Comments;

                        XmlNode Items = invoicesLine.SelectSingleNode("Item");
                        XmlNode Price = invoicesLine.SelectSingleNode("Price");
                        XmlNode TaxTotal = invoicesLine.SelectSingleNode("TaxTotal");
                        XmlNode TaxSubtotal = TaxTotal.SelectSingleNode("TaxSubtotal");
                        XmlNode TaxCategory = TaxSubtotal.SelectSingleNode("TaxCategory");

                        dr["Item_Code"] = "0";
                        dr["Item_Name"] = Items["Description"].InnerText;

                        XmlNode PackQuantity = Items.SelectSingleNode("PackQuantity ");
                        string UOM = PackQuantity.Attributes.GetNamedItem("unitCode").Value;

                        dr["UOM"] = UOM;

                        dr["Quantity"] = Convert.ToDecimal(Price["BaseQuantity"].InnerText);
                        dr["NBR_Price"] = Convert.ToDecimal(Price["PriceAmount"].InnerText);

                        dr["VAT_Amount"] = Convert.ToDecimal(TaxSubtotal["TaxAmount"].InnerText);
                        dr["SD_Rate"] = "0";
                        dr["SDAmount"] = 0;

                        dr["Currency_Code"] = ExchangeRate["SourceCurrencyCode"].InnerText;
                        dr["SubTotal"] = 0;
                        dr["Discount_Amount"] = 0;

                        ////dr["TransactionType"] = "Other";
                        dr["CompanyCode"] = CompanyCode;

                        decimal VATRate = 0;
                        VATRate = Convert.ToDecimal(TaxCategory["Percent"].InnerText);

                        dr["VAT_Rate"] = VATRate;

                        if (VATRate == 15)
                        {
                            dr["Type"] = "VAT";
                        }
                        else if (VATRate == 0)
                        {
                            dr["Type"] = "NoNVAT";
                        }
                        else
                        {
                            dr["Type"] = "OtherRate";
                        }

                        dr["Promotional_Quantity"] = "0";
                        dr["Sale_Type"] = "Credit";
                        dr["Vehicle_No"] = "NA";
                        dr["VehicleType"] = "NA";
                        dr["SD_Rate"] = "0";
                        dr["Is_Print"] = "N";
                        dr["Tender_Id"] = "0";
                        dr["Post"] = "N";
                        dr["LC_Number"] = "NA";
                        dr["CommentsD"] = "NA";
                        dr["Non_Stock"] = "N";
                        dr["Trading_MarkUp"] = "0";
                        dr["VAT_Name"] = "VAT 4.3";

                        dr["Previous_Invoice_No"] = "0";
                        dr["PreviousNBRPrice"] = "0";
                        dr["PreviousQuantity"] = "0";
                        dr["PreviousSubTotal"] = "0";
                        dr["PreviousVATAmount"] = "0";
                        dr["PreviousVATRate"] = "0";
                        dr["PreviousSD"] = "0";
                        dr["PreviousSDAmount"] = "0";
                        dr["ReasonOfReturn"] = "NA";

                        dt.Rows.Add(dr);
                    }

                }
                #endregion

                #region Comment

                ////XmlNodeList invoices = xmlDocument.GetElementsByTagName("n0:Invoice");

                ////ProductDAL productDal = new ProductDAL();

                ////foreach (XmlNode invoice in invoices)
                ////{
                ////    //////string ID = invoice.Attributes["n2:ID"].InnerText;

                ////    XmlNodeList IDs = invoice.SelectNodes("n2:ID");

                ////    string ID = invoice.SelectSingleNode("n2:ID").InnerText;

                ////    string refNo = ID;
                ////    XmlNode customer = invoice.SelectSingleNode("ADDR[@TyCd='IV']");
                ////    string customerName = customer["Name"].InnerText;
                ////    string delivaeryAdd = "";
                ////    string bin = "-";
                ////    string saleType = "NEW";
                ////    string transactionType = "Other";

                ////    string vatType = "VAT";
                ////    string vatRate = "15";
                ////    string cnDnref = "";
                ////    string branch_Code = "";
                ////    string revenueIdentifier = "";
                ////    List<string> orginInvoices = new List<string>();
                ////    decimal grandTotal = 0;

                ////    string productRefNo = "";

                ////    #region Customer Info

                ////    XmlNodeList streets = customer.SelectNodes("Street");

                ////    foreach (XmlNode st in streets)
                ////    {
                ////        if (!string.IsNullOrEmpty(st.InnerText))
                ////        {
                ////            delivaeryAdd += st.InnerText + "\n";
                ////        }
                ////    }

                ////    if (customer.SelectSingleNode("PostCd") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("PostCd").InnerText))
                ////    {
                ////        delivaeryAdd += customer.SelectSingleNode("PostCd").InnerText + " ";
                ////    }

                ////    if (customer.SelectSingleNode("CtyNm") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("CtyNm").InnerText))
                ////    {
                ////        delivaeryAdd += customer.SelectSingleNode("CtyNm").InnerText;
                ////    }

                ////    if (customer.SelectSingleNode("CtryCd") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("CtryCd").InnerText))
                ////    {
                ////        RegionInfo info = new RegionInfo(customer.SelectSingleNode("CtryCd").InnerText);

                ////        delivaeryAdd += " " + info.DisplayName.ToUpper();
                ////    }

                ////    if (customer.SelectSingleNode("RegNum") != null && !string.IsNullOrEmpty(customer.SelectSingleNode("RegNum").InnerText))
                ////    {
                ////        bin = customer.SelectSingleNode("RegNum").InnerText;
                ////    }

                ////    #endregion

                ////    string accountCode = invoice.SelectSingleNode("Acc[@TyCd='PAYER']").InnerText;
                ////    string invoiceDateTime = invoice.SelectSingleNode("Dtm[@TyCd='INV']").InnerText;

                ////    #region BranchCode

                ////    if (branch_Code.ToLower() == "hof")
                ////    {
                ////        revenueIdentifier = "A";
                ////    }
                ////    else if (branch_Code.ToLower() == "tej")
                ////    {
                ////        revenueIdentifier = "P";
                ////    }
                ////    else
                ////    {
                ////        revenueIdentifier = "R";
                ////    }

                ////    #endregion


                ////    //XmlNodeList test = invoice.SelectNodes("UNIT[@ProdSrvCd!='Z']");


                ////    XmlNode node = invoice.SelectSingleNode("//SrvA[@TyCd='ORIGIN']");

                ////    string billsyst = invoice.SelectSingleNode("REFS").SelectSingleNode("REF[@TyCd='BILLSYST']").InnerText;

                ////    #region SaleType

                ////    string type = invoice.SelectSingleNode("REFS").SelectSingleNode("REF[@TyCd='INVTYPE']").InnerText;

                ////    if (type == "N")
                ////    {
                ////        saleType = "credit";
                ////        transactionType = "credit";

                ////    }
                ////    else if (type == "D")
                ////    {
                ////        saleType = "debit";
                ////        transactionType = "debit";
                ////    }

                ////    #endregion

                ////    #region Invoice Type

                ////    string accCsh = "ACC";
                ////    if (!OrdinaryVATDesktop.IsNumber(accountCode))
                ////    {
                ////        accCsh = "CSH";
                ////    }

                ////    #endregion


                ////    #region File Name Generate

                ////    string fileName = "^" + accountCode + "^" + ID + "^" + invoiceDateTime.Replace("-", "") + "^" +
                ////                      node.SelectSingleNode("SrvACd").InnerText + "^^" + accCsh + "^^^^^" +
                ////                      invoice.SelectSingleNode("REFS").SelectSingleNode("REF[@TyCd='BILLCTRY']")
                ////                          .InnerText + "^^" +
                ////                      billsyst.Substring(0, billsyst.Length - 1) +
                ////                      "^^BIV^OB^EBILL^COPY_VTI_DAC_CO_SPN_@gendate_@gentime";

                ////    #endregion

                ////    invoiceDateTime += " 16:00";

                ////    #region line iteams
                ////    XmlNodeList lineItems = invoice.SelectNodes("UNIT[@ProdSrvCd!='Z']");

                ////    XmlNodeList lineItemsZ = invoice.SelectNodes("UNIT[@ProdSrvCd='Z']");


                ////    #region LineItemsRegular

                ////    if (lineItems != null)
                ////    {
                ////        string originInvoice = "";
                ////        foreach (XmlNode item in lineItems)
                ////        {
                ////            finalTable.Rows.Add(finalTable.NewRow());
                ////            tempTable.Rows.Add(tempTable.NewRow());

                ////            int count = finalTable.Rows.Count - 1;
                ////            originInvoice = "";

                ////            #region Master

                ////            if (string.IsNullOrEmpty(branch_Code))
                ////            {
                ////                throw new Exception("Branch Code Not Found");
                ////            }

                ////            finalTable.Rows[count]["ID"] = ID;
                ////            finalTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                ////            finalTable.Rows[count]["Customer_Name"] = customerName;
                ////            finalTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                ////            finalTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                ////            finalTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                ////            finalTable.Rows[count]["SD_Rate"] = "0";

                ////            finalTable.Rows[count]["Non_Stock"] = "N";
                ////            finalTable.Rows[count]["Trading_MarkUp"] = "0";
                ////            finalTable.Rows[count]["Currency_Code"] = "BDT";
                ////            finalTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                ////            finalTable.Rows[count]["Post"] = "Y";
                ////            finalTable.Rows[count]["DataSource"] = "IBS";
                ////            finalTable.Rows[count]["Branch_Code"] = branch_Code;

                ////            string itemCode = item.Attributes["ProdSrvCd"].InnerText;
                ////            string comments = item.Attributes["ItemNo"].InnerText;

                ////            #endregion

                ////            XmlNode itemDetails = item.SelectSingleNode("CHGLN[@LineNo='1']");
                ////            XmlNode reasonOfReturnNode = item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("FTXS");
                ////            XmlNode DtmDate = item.SelectSingleNode("Dtm[@TyCd='UNIT']");
                ////            string returnDate = DtmDate != null ? DtmDate.InnerText : "1900-01-01";
                ////            string reasonOfReturn = "";

                ////            if (reasonOfReturnNode != null)
                ////            {
                ////                reasonOfReturn = reasonOfReturnNode.SelectSingleNode("FTX[@TyCd='CLD']").InnerText;
                ////            }


                ////            string itemName = itemDetails.Attributes["Dsc"].InnerText;

                ////            #region UOM and Dollar Value Ref

                ////            List<ProductVM> products = productDal.SelectAll("0", new[] { "pr.ProductName" }, new[] { itemName }, null, null, null, connVM);

                ////            if (products != null && products.Any())
                ////            {
                ////                finalTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                ////                tempTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                ////            }
                ////            else
                ////            {
                ////                finalTable.Rows[count]["UOM"] = "Shipment";
                ////                tempTable.Rows[count]["UOM"] = "Shipment";
                ////            }


                ////            string otherRef = "<otherRef><dollarRate>" + itemDetails.SelectSingleNode("ExRate").InnerText +
                ////                              "</dollarRate>"
                ////                              + "<dollarValue>" + itemDetails.SelectSingleNode("Val[@TyCd='INV']")
                ////                                  .InnerText + "</dollarValue><fileName>" + srcFileName + "</fileName></otherRef>";

                ////            #endregion

                ////            #region Price Calculation

                ////            string qunatity = "1";
                ////            //item.SelectSingleNode("MEAS[@TyCd='EA']") == null
                ////            //? "1"
                ////            //: item.SelectSingleNode("MEAS[@TyCd='EA']").InnerText;

                ////            string weight = item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']") == null
                ////                ? "0"
                ////                : item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']").InnerText;


                ////            XmlNodeList prices = item.SelectNodes("CHGLN");

                ////            decimal onlyPrice = 0;
                ////            decimal charges = 0;

                ////            decimal vatAmount = 0;
                ////            decimal lineTotal = 0;
                ////            decimal TemplineTotal = 0;
                ////            decimal tempVatAmount = 0;

                ////            foreach (XmlNode price in prices)
                ////            {

                ////                XmlNode tax = price.SelectSingleNode("TAX");

                ////                if ((price.SelectSingleNode("TAX[@TxCd='A']") == null && price.SelectSingleNode("TAX[@TxCd='B']") == null) && tax != null) continue;


                ////                string priceAndChrgs = "0";

                ////                priceAndChrgs = tax != null
                ////                    ? tax.SelectSingleNode("AmtSubj[@TyCd='CHG']").InnerText
                ////                    : price.SelectSingleNode("Val[@TyCd='CHG']").InnerText;


                ////                if (price.Attributes["LineNo"].InnerText != "1")
                ////                {
                ////                    charges += Convert.ToDecimal(priceAndChrgs);
                ////                }
                ////                else
                ////                {
                ////                    onlyPrice += Convert.ToDecimal(priceAndChrgs);

                ////                    if (price.Attributes != null && price.Attributes["Units"] != null && price.Attributes["Units"].InnerText.Trim().StartsWith("-"))
                ////                    {
                ////                        productRefNo = price.Attributes["Dsc"].InnerText;
                ////                    }
                ////                }

                ////                if (tax != null)
                ////                {
                ////                    vatAmount += Convert.ToDecimal(tax.SelectSingleNode("Val[@TyCd='CHG']").InnerText);
                ////                }


                ////                XmlNode lineTotals = price.SelectSingleNode("TOTS");

                ////                XmlNode lineTotalxml = lineTotals.SelectSingleNode("TOT[@TyCd='LINETOT_LCL']");

                ////                lineTotal += Convert.ToDecimal(lineTotalxml.InnerText);

                ////            }


                ////            decimal
                ////                totalPrice =
                ////                    onlyPrice +
                ////                    charges; //item.SelectSingleNode("TOTS").SelectSingleNode("TOT[@TyCd='UNIT_TOT_LCL']").InnerText;

                ////            if (totalPrice == 0)
                ////            {
                ////                //throw new ArgumentException("Price cant be 0");
                ////                finalTable.Rows.RemoveAt(count);
                ////                continue;
                ////            }
                ////            decimal originnalPrice = totalPrice / Convert.ToDecimal(qunatity);
                ////            decimal TempOriginnalPrice = totalPrice / Convert.ToDecimal(qunatity);

                ////            grandTotal += (totalPrice + vatAmount);
                ////            tempVatAmount = vatAmount;


                ////            if (originnalPrice < 0)
                ////            {
                ////                originnalPrice = originnalPrice * -1;
                ////            }

                ////            if (vatAmount < 0)
                ////            {
                ////                vatAmount = vatAmount * -1;
                ////            }

                ////            TemplineTotal = lineTotal;
                ////            if (lineTotal < 0)
                ////                lineTotal *= -1;

                ////            #endregion


                ////            #region VAT Type

                ////            bool itemTax =
                ////                item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("TAX") == null ||
                ////                item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("TAX[@TxCd='B']") != null;

                ////            if (itemTax)
                ////            {
                ////                vatType = "NonVAT";
                ////                vatRate = "0";
                ////            }
                ////            else
                ////            {
                ////                vatType = "VAT";
                ////                vatRate = "15";
                ////            }

                ////            if (vatAmount == 0)
                ////            {
                ////                vatType = "NonVAT";
                ////                vatRate = "0";
                ////            }

                ////            #endregion


                ////            //decimal vatRate = 15;
                ////            //decimal priceWithOutVat = (Convert.ToDecimal(totalPrice) * (100 / (100 + vatRate)));

                ////            #region CN DN Ref

                ////            XmlNode lineItemRefs = item.SelectSingleNode("REFS");

                ////            if (lineItemRefs != null)
                ////            {
                ////                XmlNodeList orginInvoice = lineItemRefs.SelectNodes("REF[@TyCd='ORIGINVNO']");

                ////                if (orginInvoice != null)
                ////                {
                ////                    foreach (XmlNode xmlNode in orginInvoice)
                ////                    {
                ////                        if (!orginInvoices.Contains(xmlNode.InnerText))
                ////                        {
                ////                            orginInvoices.Add(xmlNode.InnerText);
                ////                        }

                ////                        originInvoice = xmlNode.InnerText;

                ////                    }
                ////                }

                ////            }

                ////            #endregion

                ////            #region Details

                ////            finalTable.Rows[count]["Item_Name"] = itemName;
                ////            finalTable.Rows[count]["Item_Code"] = "-";//itemCode;

                ////            finalTable.Rows[count]["Quantity"] = qunatity;

                ////            finalTable.Rows[count]["ExtraCharge"] = charges;
                ////            finalTable.Rows[count]["VAT_Amount"] = vatAmount;
                ////            finalTable.Rows[count]["Sale_Type"] = saleType;
                ////            finalTable.Rows[count]["TransactionType"] = transactionType;
                ////            finalTable.Rows[count]["VAT_Rate"] = vatRate;
                ////            finalTable.Rows[count]["Type"] = vatType;


                ////            finalTable.Rows[count]["LineTotal"] = lineTotal;


                ////            finalTable.Rows[count]["NBR_Price"] =
                ////                originnalPrice;

                ////            finalTable.Rows[count]["Weight"] =
                ////                weight;

                ////            finalTable.Rows[count]["CommentsD"] =
                ////                comments;

                ////            finalTable.Rows[count]["FileName"] =
                ////                fileName;

                ////            finalTable.Rows[count]["OtherRef"] =
                ////                otherRef;

                ////            finalTable.Rows[count]["CustomerBIN"] =
                ////                bin;

                ////            finalTable.Rows[count]["ReasonOfReturn"] =
                ////                reasonOfReturn;

                ////            finalTable.Rows[count]["Previous_Invoice_No"] =
                ////                originInvoice;

                ////            finalTable.Rows[count]["PreviousInvoiceDateTime"] =
                ////                returnDate;

                ////            #endregion

                ////            #region Temp Details

                ////            tempTable.Rows[count]["Item_Name"] = itemName;
                ////            tempTable.Rows[count]["Item_Code"] = "-";//itemCode;

                ////            tempTable.Rows[count]["Quantity"] = qunatity;

                ////            tempTable.Rows[count]["ExtraCharge"] = charges;
                ////            tempTable.Rows[count]["VAT_Amount"] = tempVatAmount;
                ////            tempTable.Rows[count]["Sale_Type"] = saleType;
                ////            tempTable.Rows[count]["TransactionType"] = transactionType;
                ////            tempTable.Rows[count]["VAT_Rate"] = vatRate;
                ////            tempTable.Rows[count]["Type"] = vatType;
                ////            tempTable.Rows[count]["SubTotal"] = totalPrice;

                ////            tempTable.Rows[count]["LineTotal"] = TemplineTotal;


                ////            tempTable.Rows[count]["NBR_Price"] =
                ////                TempOriginnalPrice;

                ////            tempTable.Rows[count]["Weight"] =
                ////                weight;

                ////            tempTable.Rows[count]["CommentsD"] =
                ////                comments;

                ////            tempTable.Rows[count]["FileName"] =
                ////                fileName;

                ////            tempTable.Rows[count]["OtherRef"] =
                ////                otherRef;

                ////            tempTable.Rows[count]["CustomerBIN"] =
                ////                bin;

                ////            tempTable.Rows[count]["ReasonOfReturn"] =
                ////                reasonOfReturn;


                ////            tempTable.Rows[count]["Previous_Invoice_No"] =
                ////                originInvoice;

                ////            tempTable.Rows[count]["PreviousInvoiceDateTime"] =
                ////                returnDate;

                ////            #endregion

                ////        }
                ////    }

                ////    #endregion

                ////    #region LineItemsZ

                ////    if (lineItemsZ != null)
                ////    {
                ////        string originInvoice = "";
                ////        foreach (XmlNode item in lineItemsZ)
                ////        {


                ////            XmlNode reasonOfReturnNode = item.SelectSingleNode("CHGLN[@LineNo='1']").SelectSingleNode("FTXS");
                ////            string reasonOfReturn = "";

                ////            if (reasonOfReturnNode != null)
                ////            {
                ////                reasonOfReturn = reasonOfReturnNode.SelectSingleNode("FTX[@TyCd='CLD']").InnerText;
                ////            }

                ////            XmlNode DtmDate = item.SelectSingleNode("Dtm[@TyCd='UNIT']");
                ////            string returnDate = DtmDate != null ? DtmDate.InnerText : "1900-01-01";

                ////            XmlNode itemDetails = item.SelectSingleNode("CHGLN[@LineNo='1']");


                ////            string weight = item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']") == null
                ////                ? "0"
                ////                : item.SelectSingleNode("MEAS[@TyCd='BILLWGHT']").InnerText;

                ////            #region UOM and Dollar Value Ref

                ////            //List<ProductVM> products = productDal.SelectAll("0", new[] { "pr.ProductName" }, new[] { itemName });

                ////            //if (products != null && products.Any())
                ////            //{
                ////            //    finalTable.Rows[count]["UOM"] = products.FirstOrDefault().UOM;
                ////            //}
                ////            //else
                ////            //{
                ////            //    finalTable.Rows[count]["UOM"] = "Shipment";
                ////            //}



                ////            string otherRef = "<otherRef><dollarRate>" +
                ////                              itemDetails.SelectSingleNode("ExRate").InnerText +
                ////                              "</dollarRate>"
                ////                              + "<dollarValue>" + itemDetails.SelectSingleNode("Val[@TyCd='INV']")
                ////                                  .InnerText + "</dollarValue><fileName>" + srcFileName +
                ////                              "</fileName></otherRef>";

                ////            #endregion


                ////            XmlNodeList prices = item.SelectNodes("CHGLN");


                ////            foreach (XmlNode price in prices)
                ////            {
                ////                ////var rows = chargeCodes.Select("ChargeCode = '" +
                ////                ////                   price.Attributes["ChgCd"].InnerText.ToUpper() + "'");

                ////                // !price.Attributes["ChgCd"].InnerText.ToLower().StartsWith("w")
                ////                ////if (rows.Length > 0)
                ////                ////{
                ////                ////    if (price.SelectSingleNode("TAX[@TxCd='A']") == null && price.SelectSingleNode("TAX[@TxCd='B']") == null)
                ////                ////        continue;
                ////                ////}


                ////                finalTable.Rows.Add(finalTable.NewRow());
                ////                tempTable.Rows.Add(tempTable.NewRow());

                ////                int count = finalTable.Rows.Count - 1;

                ////                #region Master

                ////                if (string.IsNullOrEmpty(branch_Code))
                ////                {
                ////                    throw new Exception("Branch Code Not Found");
                ////                }

                ////                finalTable.Rows[count]["ID"] = ID;
                ////                finalTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                ////                finalTable.Rows[count]["Customer_Name"] = customerName;
                ////                finalTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                ////                finalTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                ////                finalTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                ////                finalTable.Rows[count]["SD_Rate"] = "0";

                ////                finalTable.Rows[count]["Non_Stock"] = "N";
                ////                finalTable.Rows[count]["Trading_MarkUp"] = "0";
                ////                finalTable.Rows[count]["Currency_Code"] = "BDT";
                ////                finalTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                ////                finalTable.Rows[count]["Post"] = "Y";
                ////                finalTable.Rows[count]["Branch_Code"] = branch_Code;

                ////                finalTable.Rows[count]["DataSource"] = "IBS";
                ////                finalTable.Rows[count]["UOM"] = "Shipment";


                ////                #region Temp table data assign

                ////                tempTable.Rows[count]["ID"] = ID;
                ////                tempTable.Rows[count]["Customer_Code"] = "-";//accountCode;
                ////                tempTable.Rows[count]["Customer_Name"] = customerName;
                ////                tempTable.Rows[count]["Invoice_Date_Time"] = invoiceDateTime;
                ////                tempTable.Rows[count]["Reference_No"] = refNo + "~" + accountCode + "~" + revenueIdentifier;
                ////                tempTable.Rows[count]["Delivery_Address"] = delivaeryAdd;

                ////                tempTable.Rows[count]["SD_Rate"] = "0";

                ////                tempTable.Rows[count]["Non_Stock"] = "N";
                ////                tempTable.Rows[count]["Trading_MarkUp"] = "0";
                ////                tempTable.Rows[count]["Currency_Code"] = "BDT";
                ////                tempTable.Rows[count]["VAT_Name"] = "VAT 4.3";
                ////                tempTable.Rows[count]["Post"] = "Y";
                ////                tempTable.Rows[count]["Branch_Code"] = branch_Code;

                ////                tempTable.Rows[count]["DataSource"] = "IBS";
                ////                tempTable.Rows[count]["UOM"] = "Shipment";

                ////                #endregion


                ////                string itemCode = item.Attributes["ProdSrvCd"].InnerText;
                ////                string comments = item.Attributes["ItemNo"].InnerText;

                ////                #endregion


                ////                decimal onlyPrice = 0;
                ////                decimal charges = 0;
                ////                string qunatity = "1";

                ////                decimal vatAmount = 0;
                ////                decimal tempVatAmount = 0;

                ////                XmlNode tax = price.SelectSingleNode("TAX");

                ////                string priceAndChrgs = "0";


                ////                string itemName = price.Attributes["Dsc"].InnerText;

                ////                priceAndChrgs = tax != null
                ////                    ? tax.SelectSingleNode("AmtSubj[@TyCd='CHG']").InnerText
                ////                    : price.SelectSingleNode("Val[@TyCd='CHG']").InnerText;


                ////                onlyPrice = Convert.ToDecimal(priceAndChrgs);

                ////                if (tax != null)
                ////                {
                ////                    vatAmount += Convert.ToDecimal(tax.SelectSingleNode("Val[@TyCd='CHG']").InnerText);
                ////                }

                ////                XmlNode lineTotals = price.SelectSingleNode("TOTS");

                ////                XmlNode lineTotalxml = lineTotals.SelectSingleNode("TOT[@TyCd='LINETOT_LCL']");

                ////                decimal lineTotal = Convert.ToDecimal(lineTotalxml.InnerText);
                ////                decimal tempLineTotal = Convert.ToDecimal(lineTotalxml.InnerText);

                ////                decimal
                ////                    totalPrice =
                ////                        onlyPrice +
                ////                        charges; //item.SelectSingleNode("TOTS").SelectSingleNode("TOT[@TyCd='UNIT_TOT_LCL']").InnerText;

                ////                if (totalPrice == 0)
                ////                {
                ////                    finalTable.Rows.RemoveAt(count);
                ////                    continue;
                ////                }
                ////                decimal originnalPrice = totalPrice / Convert.ToDecimal(qunatity);
                ////                decimal TemporiginnalPrice = totalPrice / Convert.ToDecimal(qunatity);

                ////                grandTotal += (totalPrice + vatAmount);
                ////                tempVatAmount = vatAmount;

                ////                if (originnalPrice < 0)
                ////                    originnalPrice = originnalPrice * -1;

                ////                if (vatAmount < 0)
                ////                    vatAmount = vatAmount * -1;

                ////                if (lineTotal < 0)
                ////                    lineTotal *= -1;


                ////                if (price.Attributes != null && price.Attributes["Units"] != null && price.Attributes["Units"].InnerText.Trim().StartsWith("-"))
                ////                {
                ////                    productRefNo = price.Attributes["Dsc"].InnerText;
                ////                }

                ////                #region VAT Type

                ////                bool itemTax = price.SelectSingleNode("TAX") == null ||
                ////                               price.SelectSingleNode("TAX[@TxCd='B']") != null;

                ////                if (itemTax)
                ////                {
                ////                    vatType = "NonVAT";
                ////                    vatRate = "0";
                ////                }
                ////                else
                ////                {
                ////                    vatType = "VAT";
                ////                    vatRate = "15";
                ////                }

                ////                if (vatAmount == 0)
                ////                {
                ////                    vatType = "NonVAT";
                ////                    vatRate = "0";
                ////                }

                ////                #endregion

                ////                #region CN DN Ref

                ////                XmlNode lineItemRefs = item.SelectSingleNode("REFS");

                ////                if (lineItemRefs != null)
                ////                {
                ////                    XmlNodeList orginInvoice = lineItemRefs.SelectNodes("REF[@TyCd='ORIGINVNO']");

                ////                    if (orginInvoice != null)
                ////                    {
                ////                        foreach (XmlNode xmlNode in orginInvoice)
                ////                        {
                ////                            if (!orginInvoices.Contains(xmlNode.InnerText))
                ////                            {
                ////                                orginInvoices.Add(xmlNode.InnerText);
                ////                            }

                ////                            originInvoice = xmlNode.InnerText;

                ////                        }
                ////                    }

                ////                }

                ////                #endregion

                ////                #region Details

                ////                finalTable.Rows[count]["Item_Name"] = itemName;
                ////                finalTable.Rows[count]["Item_Code"] = "-";//itemCode;

                ////                finalTable.Rows[count]["Quantity"] = qunatity;

                ////                finalTable.Rows[count]["ExtraCharge"] = charges;
                ////                finalTable.Rows[count]["VAT_Amount"] = vatAmount;
                ////                finalTable.Rows[count]["SubTotal"] = totalPrice;

                ////                finalTable.Rows[count]["Sale_Type"] = saleType;
                ////                finalTable.Rows[count]["TransactionType"] = transactionType;
                ////                finalTable.Rows[count]["VAT_Rate"] = vatRate;
                ////                finalTable.Rows[count]["Type"] = vatType;

                ////                finalTable.Rows[count]["LineTotal"] = lineTotal;

                ////                finalTable.Rows[count]["NBR_Price"] =
                ////                    originnalPrice;

                ////                finalTable.Rows[count]["Weight"] =
                ////                    weight;

                ////                finalTable.Rows[count]["CommentsD"] =
                ////                    comments;

                ////                finalTable.Rows[count]["FileName"] =
                ////                    fileName;

                ////                finalTable.Rows[count]["OtherRef"] =
                ////                    otherRef;

                ////                finalTable.Rows[count]["CustomerBIN"] =
                ////                    bin;

                ////                finalTable.Rows[count]["ReasonOfReturn"] =
                ////                    reasonOfReturn;

                ////                finalTable.Rows[count]["Previous_Invoice_No"] =
                ////                    originInvoice;
                ////                finalTable.Rows[count]["PreviousInvoiceDateTime"] =
                ////                    returnDate;

                ////                #endregion

                ////                #region Temp table

                ////                tempTable.Rows[count]["Item_Name"] = itemName;
                ////                tempTable.Rows[count]["Item_Code"] = "-";//itemCode;

                ////                tempTable.Rows[count]["Quantity"] = qunatity;

                ////                tempTable.Rows[count]["ExtraCharge"] = charges;
                ////                tempTable.Rows[count]["VAT_Amount"] = tempVatAmount;
                ////                tempTable.Rows[count]["SubTotal"] = totalPrice;

                ////                tempTable.Rows[count]["Sale_Type"] = saleType;
                ////                tempTable.Rows[count]["TransactionType"] = transactionType;
                ////                tempTable.Rows[count]["VAT_Rate"] = vatRate;
                ////                tempTable.Rows[count]["Type"] = vatType;

                ////                tempTable.Rows[count]["LineTotal"] = tempLineTotal;


                ////                tempTable.Rows[count]["NBR_Price"] =
                ////                    TemporiginnalPrice;

                ////                tempTable.Rows[count]["Weight"] =
                ////                    weight;

                ////                tempTable.Rows[count]["CommentsD"] =
                ////                    comments;

                ////                tempTable.Rows[count]["FileName"] =
                ////                    fileName;

                ////                tempTable.Rows[count]["OtherRef"] =
                ////                    otherRef;

                ////                tempTable.Rows[count]["CustomerBIN"] =
                ////                    bin;

                ////                tempTable.Rows[count]["ReasonOfReturn"] =
                ////                    reasonOfReturn;

                ////                tempTable.Rows[count]["Previous_Invoice_No"] =
                ////                    originInvoice;

                ////                tempTable.Rows[count]["PreviousInvoiceDateTime"] =
                ////                    returnDate;

                ////                #endregion

                ////            }

                ////        }
                ////    }

                ////    #endregion

                ////    #endregion

                ////    cnDnref = string.Join(",", orginInvoices);

                ////    #region Checking Transaction Type Logic Based on CashBD Account

                ////    if (accountCode.ToLower().StartsWith("cashbd"))
                ////    {


                ////        if (!string.IsNullOrEmpty(productRefNo))
                ////        {
                ////            DataRow[] rows = finalTable.Select("Item_Name= '" + productRefNo + "'");

                ////            if (rows.Length > 1)
                ////            {
                ////                CommonDAL commonDal = new CommonDAL();

                ////                ////tempTable = commonDal.GetCumulativeValue(tempTable, null, null, connVM);

                ////                tempTable.Columns.Add(new DataColumn()
                ////                {
                ////                    ColumnName = "OtherRef",
                ////                    DefaultValue = finalTable.Rows[0]["OtherRef"].ToString()
                ////                });

                ////                finalTable = tempTable.Copy();

                ////            }
                ////        }

                ////        if (grandTotal < 0)
                ////        {
                ////            saleType = "credit";
                ////            transactionType = "credit";

                ////            finalTable.Columns.Remove("TransactionType");
                ////            finalTable.Columns.Remove("Sale_Type");

                ////            finalTable.Columns.Add(new DataColumn() { ColumnName = "TransactionType", DefaultValue = transactionType });
                ////            finalTable.Columns.Add(new DataColumn() { ColumnName = "Sale_Type", DefaultValue = saleType });

                ////        }
                ////        else if (grandTotal > 0 && !string.IsNullOrEmpty(productRefNo))
                ////        {
                ////            saleType = "debit";
                ////            transactionType = "debit";

                ////            finalTable.Columns.Remove("TransactionType");
                ////            finalTable.Columns.Remove("Sale_Type");

                ////            finalTable.Columns.Add(new DataColumn() { ColumnName = "TransactionType", DefaultValue = transactionType });
                ////            finalTable.Columns.Add(new DataColumn() { ColumnName = "Sale_Type", DefaultValue = saleType });
                ////        }
                ////    }

                ////    #endregion

                ////    finalTable.Columns.Add(new DataColumn() { DefaultValue = cnDnref, ColumnName = "CNDNRef" });
                ////}

                #endregion

            }

            catch (ArgumentException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
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
            table.Columns.Add("VAT_Name");

            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "SubTotal" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "ExtraCharge" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "FileName" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "OtherRef" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "VAT_Amount" });
            table.Columns.Add(new DataColumn { DefaultValue = "0", ColumnName = "LineTotal" });
            table.Columns.Add(new DataColumn { DefaultValue = "Other", ColumnName = "TransactionType" });
            ////table.Columns.Add(new DataColumn { DefaultValue = "N", ColumnName = "IsPDFGenerated" });
            table.Columns.Add(new DataColumn { DefaultValue = "", ColumnName = "Previous_Invoice_No" });
            table.Columns.Add(new DataColumn { DefaultValue = "1900-01-01", ColumnName = "PreviousInvoiceDateTime" });
            table.Columns.Add(new DataColumn { DefaultValue = "N", ColumnName = "Is_Print" });

            return table;

        }

        private void stopBulk_Click(object sender, EventArgs e)
        {
            try
            {
               
                if (timer1.Enabled)
                {
                    timer1.Stop();
                }

                MessageBox.Show("Process Stopped");

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormSaleImportErnst", "stopBulk_Click", exception.Message + "\n" + exception.StackTrace);
                MessageBox.Show(exception.Message);
            }
        }

        private void FormSaleImportErnst_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

                if (timer1.Enabled)
                {
                    timer1.Stop();
                }

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormSaleImportErnst", "FormSaleImportErnst_FormClosing", exception.Message + "\n" + exception.StackTrace);
                MessageBox.Show(exception.Message);
            }
        }

        private void FormSaleImportErnst_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

                if (timer1.Enabled)
                {
                    timer1.Stop();
                }

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormSaleImportErnst", "FormSaleImportErnst_FormClosed", exception.Message + "\n" + exception.StackTrace);
                MessageBox.Show(exception.Message);
            }
        }

        public SysDBInfoVMTemp Authenticate(string CompanyCode)
        {
            string BIN = "";

            if (CompanyCode.ToLower() == "PB23".ToLower())
            {
                BIN = "000000539-0003";
            }
            else if (CompanyCode.ToLower() == "PBBO".ToLower())
            {
                BIN = "002599409-0103";
            }
            else
            {
                BIN = "002599418-0103";
            }

            var enBin = Converter.DESEncrypt(DBConstant.PassPhrase, DBConstant.EnKey, BIN.Trim());

            var sysInfo = new CompanyInformationDAL().SelectAll(null, new[] { "Bin" }, new[] { enBin }, null, null, connVM).FirstOrDefault();

            if (sysInfo == null)
            {
                throw new Exception("BIN does not exist");
            }

            var dbName = sysInfo.DatabaseName; //Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, sysInfo.DatabaseName);

            SysDBInfoVM.SysdataSource = connVM.SysdataSource;
            SysDBInfoVM.SysPassword = connVM.SysPassword;
            SysDBInfoVM.SysUserName = connVM.SysUserName;
            DatabaseInfoVM.DatabaseName = dbName;

            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp()
            {
                SysdataSource = connVM.SysdataSource,
                SysPassword = connVM.SysPassword,
                SysUserName = connVM.SysUserName,
            };

            //////NewconnVM = connVM;
            NewconnVM.SysDatabaseName = dbName;

            return NewconnVM;

        }


    }
}
