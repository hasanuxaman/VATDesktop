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
using SymphonySofttech.Utilities;

namespace VATClient
{
    public partial class FormSaleImportmMarico : Form
    {
        #region Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        string[] sqlResults = new string[6];
        string loadedTable;
        private int _saleRowCount = 0;
        public bool isFileSelected = false;
        public string preSelectTable;
        public string transactionType;
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

        public bool IsCDN = false;

        public static bool FreeSlot = true;
        List<string> SaleFile = new List<string>();
        List<string> FactorySaleFile = new List<string>();

        List<string> PurchaseFile = new List<string>();
        List<string> FactoryPurchaseFile = new List<string>();

        List<string> TransferFile = new List<string>();
        List<string> FactoryTransferFile = new List<string>();

        List<string> FactoryContractManufacturingFile = new List<string>();
        List<string> FactoryIssueFile = new List<string>();

        bool isSymphony = false;

        #endregion

        public FormSaleImportmMarico()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }

        public static string SelectOne(string transactionType)
        {
            FormSaleImportmMarico form = new FormSaleImportmMarico();
            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            // dgvLoadedTable.ReadOnly = true;

            //isSymphony = true;

            isSymphony = false;

            filePathLoad();

        }

        private void filePathLoad()
        {
            if (isSymphony)
            {

                User = "VATClient";
                Pass = "S123456_";

                #region For Symphony path

                SaleWorkPath = @"";
                SaleLocal = @"E:\Sales";
                SaleHoldPath = @"E:\Sales\Hold";
                SalePostedPath = @"E:\Sales\Posted";
                SaleLocalStorePath = @"E:\Sales\Work\";

                PurchaseWorkPath = @"";
                Purchase = @"E:\Purchases";
                PurchaseHoldPath = @"E:\Purchases\Hold";
                PurchasePostedPath = @"E:\Purchases\Posted";
                PurchaseLocalStorePath = @"E:\Purchases\Work\";

                TransferWorkPath = @"";
                Transfer = @"E:\Transfers";
                TransferHoldPath = @"E:\Transfers\Hold\";
                TransferPostedPath = @"E:\Transfers\Posted";
                TransferLocalStorePath = @"E:\Transfers\Work\";

                FactorySaleWorkPath = @"";
                FactorySaleLocal = @"E:\FactorySales";
                FactorySaleHoldPath = @"E:\FactorySales\Hold";
                FactorySalePostedPath = @"E:\FactorySales\Posted";
                FactorySaleLocalStorePath = @"E:\FactorySales\Work\";

                FactoryTransferWorkPath = @"";
                FactoryTransfer = @"E:\FactoryTransfers";
                FactoryTransferHoldPath = @"E:\FactoryTransfers\Hold\";
                FactoryTransferPostedPath = @"E:\FactoryTransfers\Posted";
                FactoryTransferLocalStorePath = @"E:\FactoryTransfers\Work\";

                ContractManufacturingWorkPath = @"";
                ContractManufacturing = @"E:\ContractManufacturing";
                ContractManufacturingHoldPath = @"E:\ContractManufacturing\Hold\";
                ContractManufacturingPostedPath = @"E:\ContractManufacturing\Posted";
                ContractManufacturingStorePath = @"E:\ContractManufacturing\Work\";

                FactoryPurchaseWorkPath = @"";
                FactoryPurchase = @"E:\FactoryPurchases";
                FactoryPurchaseHoldPath = @"E:\FactoryPurchases\Hold";
                FactoryPurchasePostedPath = @"E:\FactoryPurchases\Posted";
                FactoryPurchaseLocalStorePath = @"E:\FactoryPurchases\Work\";

                FactoryIssueWorkPath = @"";
                FactoryIssue = @"E:\FactoryIssue";
                FactoryIssueHoldPath = @"E:\FactoryIssue\Hold";
                FactoryIssuePostedPath = @"E:\FactoryIssue\Posted";
                FactoryIssueLocalStorePath = @"E:\FactoryIssue\Work\";

                #endregion

            }
            else
            {

                User = "mblevatprd";//"MBLEVATUAT";
                Pass = "Jfk34#@2022";

                #region For Marico Path

                SaleWorkPath = @"";
                SaleLocal = @"F:\Sales";
                SaleHoldPath = @"F:\Sales\Hold";
                SalePostedPath = @"F:\Sales\Posted";
                SaleLocalStorePath = @"F:\Sales\Work\";

                PurchaseWorkPath = @"";
                Purchase = @"F:\Purchases";
                PurchaseHoldPath = @"F:\Purchases\Hold";
                PurchasePostedPath = @"F:\Purchases\Posted";
                PurchaseLocalStorePath = @"F:\Purchases\Work\";

                TransferWorkPath = @"";
                Transfer = @"F:\Transfers";
                TransferHoldPath = @"F:\Transfers\Hold\";
                TransferPostedPath = @"F:\Transfers\Posted";
                TransferLocalStorePath = @"F:\Transfers\Work\";

                FactorySaleWorkPath = @"";
                FactorySaleLocal = @"F:\FactorySales";
                FactorySaleHoldPath = @"F:\FactorySales\Hold";
                FactorySalePostedPath = @"F:\FactorySales\Posted";
                FactorySaleLocalStorePath = @"F:\FactorySales\Work\";

                FactoryTransferWorkPath = @"";
                FactoryTransfer = @"F:\FactoryTransfers";
                FactoryTransferHoldPath = @"F:\FactoryTransfers\Hold\";
                FactoryTransferPostedPath = @"F:\FactoryTransfers\Posted";
                FactoryTransferLocalStorePath = @"F:\FactoryTransfers\Work\";

                ContractManufacturingWorkPath = @"";
                ContractManufacturing = @"F:\ContractManufacturing";
                ContractManufacturingHoldPath = @"F:\ContractManufacturing\Hold\";
                ContractManufacturingPostedPath = @"F:\ContractManufacturing\Posted";
                ContractManufacturingStorePath = @"F:\ContractManufacturing\Work\";

                FactoryPurchaseWorkPath = @"";
                FactoryPurchase = @"F:\FactoryPurchases";
                FactoryPurchaseHoldPath = @"F:\FactoryPurchases\Hold";
                FactoryPurchasePostedPath = @"F:\FactoryPurchases\Posted";
                FactoryPurchaseLocalStorePath = @"F:\FactoryPurchases\Work\";

                FactoryIssueWorkPath = @"";
                FactoryIssue = @"F:\FactoryIssue";
                FactoryIssueHoldPath = @"F:\FactoryIssue\Hold";
                FactoryIssuePostedPath = @"F:\FactoryIssue\Posted";
                FactoryIssueLocalStorePath = @"F:\FactoryIssue\Work\";


                #endregion

            }

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

        public void GetFile(List<string> File)
        {
            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp();

            int loopCounter = 100;

            if (File.Count < loopCounter)
            {
                loopCounter = File.Count;
            }
            for (var index = 0; index < loopCounter; index++)
            {
                string file = SaleWorkPath + "/" + File[index];
                string WorkFile = File[index];
                string destinationFile = SalePostedPath + "/" + File[index];
                string destinationHoldFile = SaleHoldPath + "/" + File[index];
                string importID = "";
                try
                {
                    NewconnVM = Authenticate("RBBH");

                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(SaleWorkPath + "/" + File[index]);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.Credentials = new NetworkCredential(User, Pass);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string xml = reader.ReadToEnd();
                    response.Close();
                    xml = xml.Replace("&", "And");

                    DataTable dtsale = GetTableFromXML(xml);
                    dtsale = FormatSaleData(dtsale);
                    TableValidation(dtsale, File[index]);
                    SaleDAL saleDal = new SaleDAL();

                    importID = string.Join(",", dtsale
                       .DefaultView
                       .ToTable(true, "ID")
                       .Rows.Cast<DataRow>()
                       .Select(x => x["ID"].ToString()));

                    dtsale.Columns["Vehicle_Type"].ColumnName = "VehicleType";
                    dtsale.Columns["WAERK"].ColumnName = "Currency_Code";
                    dtsale.Columns.Remove("LASNO");
                    dtsale.Columns.Remove("PRODUCTDESCRIPTION");
                    dtsale.Columns.Remove("FKDAT");
                    dtsale.Columns.Remove("ERZET");

                    saleDal.SaveAndProcess(dtsale, () => { }, Program.BranchId, "", NewconnVM);

                    MoveFile(file, destinationFile, WorkFile, SaleLocalStorePath, importID, "Sale");
                }
                catch (Exception e)
                {
                    try
                    {

                        CommonDAL _cdal = new CommonDAL();
                        MoveFile(file, destinationHoldFile, WorkFile, SaleLocalStorePath, importID, "Sale");

                        ErrorLogVM vm = new ErrorLogVM();
                        List<ErrorLogVM> vms = new List<ErrorLogVM>();


                        FileLogger.Log("MARICO", "Getfiles", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                        vm.ImportId = importID;
                        vm.FileName = WorkFile;
                        vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                        vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        vm.SourceName = "MARICO";
                        vm.ActionName = "Getfiles";
                        vm.TransactionName = "Sale";
                        vms.Add(vm);

                        DataTable dt = vms.ToList().ToDataTable();
                        _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                    }
                    catch
                    {

                    }
                }
            }

        }

        public static DataTable FormatSaleData(DataTable table)
        {
            #region Columns


            Dictionary<string, string> SaleColumns = new Dictionary<string, string>()
            {
                {"ID".ToLower(),"ID"},
                {"Branch_Code".ToLower(),"Branch_Code"},
                {"CustomerGroup".ToLower(),"CustomerGroup"},
                {"Customer_Name".ToLower(),"Customer_Name"},
                {"Customer_Code".ToLower(),"Customer_Code"},
                {"Delivery_Address".ToLower(),"Delivery_Address"},
                {"Invoice_Date_Time".ToLower(),"Invoice_Date_Time"},
                {"Delivery_Date_Time".ToLower(),"Delivery_Date_Time"},
                {"Reference_No".ToLower(),"Reference_No"},
                {"Comments".ToLower(),"Comments"},
                {"Sale_Type".ToLower(),"Sale_Type"},
                {"Previous_Invoice_No".ToLower(),"Previous_Invoice_No"},
                {"Is_Print".ToLower(),"Is_Print"},
                {"Tender_Id".ToLower(),"Tender_Id"},
                {"Post".ToLower(),"Post"},
                {"LC_Number".ToLower(),"LC_Number"},
                {"Currency_Code".ToLower(),"Currency_Code"},
                {"CommentsD".ToLower(),"CommentsD"},
                {"Item_Code".ToLower(),"Item_Code"},
                {"Quantity".ToLower(),"Quantity"},
                {"Item_Name".ToLower(),"Item_Name"},
                {"NBR_Price".ToLower(),"NBR_Price"},
                {"UOM".ToLower(),"UOM"},
                {"VAT_Rate".ToLower(),"VAT_Rate"},
                {"SD_Rate".ToLower(),"SD_Rate"},
                {"Non_Stock".ToLower(),"Non_Stock"},
                {"Type".ToLower(),"Type"},
                {"Discount_Amount".ToLower(),"Discount_Amount"},
                {"Promotional_Quantity".ToLower(),"Promotional_Quantity"},
                {"Promotion_Quantity".ToLower(),"Promotional_Quantity"},
                {"VAT_Name".ToLower(),"VAT_Name"},
                {"SubTotal".ToLower(),"SubTotal"},
                {"Vehicle_No".ToLower(),"Vehicle_No"},
                {"ExpDescription".ToLower(),"ExpDescription"},
                {"ExpQuantity".ToLower(),"ExpQuantity"},
                {"ExpGrossWeight".ToLower(),"ExpGrossWeight"},
                {"ExpNetWeight".ToLower(),"ExpNetWeight"},
                {"ExpNumberFrom".ToLower(),"ExpNumberFrom"},
                {"ExpNumberTo".ToLower(),"ExpNumberTo"},
                {"SDAmount".ToLower(),"SDAmount"},
                {"VAT_Amount".ToLower(),"VAT_Amount"},
                {"trading_markup".ToLower(),"Trading_MarkUp"},
                {"ReturnId".ToLower(),"ReturnId"},
                {"Second_Unit".ToLower(),"Weight"},
                {"transactiontype".ToLower(),"TransactionType"},
                {"Vehicle_Type".ToLower(),"Vehicle_Type"},

                {"PreviousNBRPrice".ToLower(),"PreviousNBRPrice"},
                {"PreviousQuantity".ToLower(),"PreviousQuantity"},
                {"PreviousSubTotal".ToLower(),"PreviousSubTotal"},
                {"PreviousSD".ToLower(),"PreviousSD"},
                {"PreviousSDAmount".ToLower(),"PreviousSDAmount"},
                {"PreviousVATRate".ToLower(),"PreviousVATRate"},
                {"PreviousVATAmount".ToLower(),"PreviousVATAmount"},
                {"PreviousUOM".ToLower(),"PreviousUOM"},
                {"ReasonOfReturn".ToLower(),"ReasonOfReturn"},
                {"PreviousInvoiceDateTime".ToLower(),"PreviousInvoiceDateTime"},
                {"CustomerBIN".ToLower(),"CustomerBIN"},
                {"CustomerAddress".ToLower(),"CustomerAddress"},
                {"UserName".ToLower(),"UserName"},
                {"FKDAT".ToLower(),"FKDAT"},
                {"ERZET".ToLower(),"ERZET"},
                {"LASNO".ToLower(),"LASNO"},
                {"WAERK".ToLower(),"WAERK"},
                {"PRODUCTDESCRIPTION".ToLower(),"PRODUCTDESCRIPTION"},
                {"OtherRef".ToLower(),"OtherRef"},
                {"CONVERSION_FACTOR_TO_NOS".ToLower(),"CONVERSION_FACTOR_TO_NOS"},
                {"PREVIOUS_CONVERSION_FACTOR_TO_NOS".ToLower(),"PREVIOUS_CONVERSION_FACTOR_TO_NOS"},
                
            };

            #endregion
            try
            {
                var finalTable = new DataTable();

                foreach (DataColumn col in table.Columns)
                {
                    var clName = col.ColumnName.ToLower();

                    if (!SaleColumns.Keys.Contains(clName))
                        continue;

                    clName = SaleColumns[clName];

                    //table.Columns[col.ColumnName].ColumnName = clName;

                    finalTable.Columns.Add(clName);

                }

                foreach (DataRow row in table.Rows)
                {
                    var data = row.ItemArray;
                    finalTable.Rows.Add(data);
                }

                if (!finalTable.Columns.Contains("CustomerBIN"))
                {
                    var columnName = new DataColumn("CustomerBIN") { DefaultValue = "XNAX" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Invoice_Date_Time"))
                {
                    var columnName = new DataColumn("Invoice_Date_Time") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }

                if (!finalTable.Columns.Contains("PreviousNBRPrice"))
                {
                    var columnName = new DataColumn("PreviousNBRPrice") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Weight"))
                {
                    var columnName = new DataColumn("Weight") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }
                foreach (DataRow rows in finalTable.Rows)
                {
                    rows["UOM"] = "Pcs";
                    rows["Weight"] = rows["CONVERSION_FACTOR_TO_NOS"].ToString() == "1" ? "0" : rows["Quantity"].ToString();


                    if (!string.IsNullOrWhiteSpace(rows["CONVERSION_FACTOR_TO_NOS"].ToString()) && rows["UOM"].ToString().ToLower() != "nos")
                    {
                        rows["Quantity"] = Convert.ToDecimal(rows["Quantity"]) * Convert.ToDecimal(rows["CONVERSION_FACTOR_TO_NOS"]);
                        rows["NBR_Price"] = Convert.ToDecimal(rows["NBR_Price"]) / Convert.ToDecimal(rows["CONVERSION_FACTOR_TO_NOS"]);
                    }

                    if (rows["Sale_Type"].ToString() == "ZBRE")
                    {
                        rows["PreviousUOM"] = "Pcs";

                        rows["PreviousNBRPrice"] = Convert.ToDecimal(rows["PreviousSubTotal"]) / (Convert.ToDecimal(rows["PreviousQuantity"]) == 0 ? 1 : Convert.ToDecimal(rows["PreviousQuantity"]));


                        if (!string.IsNullOrWhiteSpace(rows["PREVIOUS_CONVERSION_FACTOR_TO_NOS"].ToString()) && rows["PreviousUOM"].ToString().ToLower() != "nos")
                        {
                            if (rows["PREVIOUS_CONVERSION_FACTOR_TO_NOS"].ToString() == "0")
                            {
                                rows["PreviousQuantity"] = Convert.ToDecimal(rows["PreviousQuantity"]) * Convert.ToDecimal(rows["CONVERSION_FACTOR_TO_NOS"]);
                                rows["PreviousNBRPrice"] = Convert.ToDecimal(rows["PreviousNBRPrice"]) / Convert.ToDecimal(rows["CONVERSION_FACTOR_TO_NOS"]);
                            }
                            else
                            {
                                rows["PreviousQuantity"] = Convert.ToDecimal(rows["PreviousQuantity"]) * Convert.ToDecimal(rows["PREVIOUS_CONVERSION_FACTOR_TO_NOS"]);
                                rows["PreviousNBRPrice"] = Convert.ToDecimal(rows["PreviousNBRPrice"]) / Convert.ToDecimal(rows["PREVIOUS_CONVERSION_FACTOR_TO_NOS"]);
                            }
                        }
                    }

                    if (finalTable.Columns.Contains("FKDAT"))
                    {
                        if (finalTable.Columns.Contains("PreviousInvoiceDateTime"))
                        {
                            string pattern = "dd.MM.yyyy HH:mm:ss";
                            DateTime parsedDate;
                            var a = rows["PreviousInvoiceDateTime"].ToString();
                            DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                            rows["PreviousInvoiceDateTime"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (rows["PreviousInvoiceDateTime"].ToString() == "0001-01-01 00:00:00")
                        {
                            rows["PreviousInvoiceDateTime"] = "1900-01-01 00:00:00";
                        }
                    }

                    if (rows["PreviousInvoiceDateTime"].ToString() == "")
                    {
                        rows["PreviousInvoiceDateTime"] = "1900-01-01 00:00:00";

                    }

                    if (rows["Sale_Type"].ToString() == "ZBFD")
                    {
                        rows["Sale_Type"] = "New";
                        rows["TransactionType"] = "Other";
                    }

                    else if (rows["Sale_Type"].ToString() == "ZBRE")
                    {
                        rows["Sale_Type"] = "Credit";
                        rows["TransactionType"] = "Credit";
                    }
                    if (finalTable.Columns.Contains("FKDAT") && finalTable.Columns.Contains("ERZET"))
                    {
                        string pattern = "dd.MM.yyyy HHmmss";
                        DateTime parsedDate;
                        var a = rows["FKDAT"].ToString() + " " + rows["ERZET"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["Invoice_Date_Time"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                finalTable.Columns.Remove("CONVERSION_FACTOR_TO_NOS");
                finalTable.Columns.Remove("Previous_Conversion_Factor_To_Nos");
                return finalTable;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static DataTable FormatPurchaseData(DataTable table)
        {
            #region Columns

            Dictionary<string, string> SaleColumns = new Dictionary<string, string>()
            {
                {"ID".ToLower(),"ID"},
                {"Branch_Code".ToLower(),"Branch_Code"},
                {"Vendor_Code".ToLower(),"Vendor_Code"},
                {"Vendor_Name".ToLower(),"Vendor_Name"},
                {"Referance_No".ToLower(),"Referance_No"},
                {"LC_No".ToLower(),"LC_No"},
                {"BE_Number".ToLower(),"BE_Number"},
                {"Invoice_Date".ToLower(),"Invoice_Date"},
                {"Receive_Date".ToLower(),"Receive_Date"},
                {"RebateDate".ToLower(),"RebateDate"},
                {"Post".ToLower(),"Post"},
                {"With_VDS".ToLower(),"With_VDS"},
                {"Comments".ToLower(),"Comments"},
                {"Transaction_Type".ToLower(),"Transaction_Type"},
                {"Custom_House".ToLower(),"Custom_House"},
                {"Item_Name".ToLower(),"Item_Name"},
                {"Item_Code".ToLower(),"Item_Code"},
                {"Product_Group".ToLower(),"Product_Group"},
                {"Quantity".ToLower(),"Quantity"},
                {"Total_Price".ToLower(),"Total_Price"},
                {"UOM".ToLower(),"UOM"},
                {"Type".ToLower(),"Type"},
                {"FixedVATRebate".ToLower(),"FixedVATRebate"},
                {"Rebate_Rate".ToLower(),"Rebate_Rate"},
                {"SD_Amount".ToLower(),"SD_Amount"},
                {"VAT_Amount".ToLower(),"VAT_Amount"},
                {"VDSAmount".ToLower(),"VDSAmount"},
                {"CnF_Amount".ToLower(),"CnF_Amount"},
                {"Insurance_Amoun".ToLower(),"Insurance_Amoun"},
                {"Assessable_Value".ToLower(),"Assessable_Value"},
                {"CD_Amount".ToLower(),"CD_Amount"},
                {"RD_Amount".ToLower(),"RD_Amount"},
                {"AITAmount".ToLower(),"AITAmount"},
                {"AT_Amount".ToLower(),"AT_Amount"},
                {"Invoice_Value".ToLower(),"Invoice_Value"},
                {"Exchange_Rate".ToLower(),"Exchange_Rate"},
                {"Currency".ToLower(),"Currency"},
                {"Others_Amount".ToLower(),"Others_Amount"},
                {"Remarks".ToLower(),"Remarks"},
                {"IsRebate".ToLower(),"IsRebate"},
                {"CONVERSION_FACTOR_TO_NOS".ToLower(),"CONVERSION_FACTOR_TO_NOS"},
 
            };

            #endregion
            try
            {
                var finalTable = new DataTable();

                foreach (DataColumn col in table.Columns)
                {
                    var clName = col.ColumnName.ToLower();

                    if (!SaleColumns.Keys.Contains(clName))
                        continue;

                    clName = SaleColumns[clName];

                    //table.Columns[col.ColumnName].ColumnName = clName;

                    finalTable.Columns.Add(clName);

                }

                foreach (DataRow row in table.Rows)
                {
                    var data = row.ItemArray;
                    finalTable.Rows.Add(data);
                }
                if (!finalTable.Columns.Contains("Total_Price"))
                {
                    var columnName = new DataColumn("Total_Price") { DefaultValue = 0 };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("VAT_Amount"))
                {
                    var columnName = new DataColumn("VAT_Amount") { DefaultValue = 0 };
                    finalTable.Columns.Add(columnName);
                }

                ProductDAL dal = new ProductDAL();

                foreach (DataRow rows in finalTable.Rows)
                {
                    if (string.IsNullOrWhiteSpace(rows["Conversion_Factor_To_Nos"].ToString()))
                    {
                        rows["UOM"] = "Pcs";
                    }
                    else
                    {
                        rows["UOM"] = "Pcs";
                        rows["Quantity"] = Convert.ToDecimal(rows["Quantity"]) * Convert.ToDecimal(rows["CONVERSION_FACTOR_TO_NOS"]);
                    }

                    rows["Type"] = "VAT";
                    rows["Post"] = "N";

                    var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { rows["Item_Code"].ToString() });

                    ProductVM vm = vms.FirstOrDefault();

                    if (vm == null)
                    {
                        try
                        {
                            CommonDAL com = new CommonDAL();
                            CommonImportDAL cImport = new CommonImportDAL();
                            cImport.FindItemId(rows["Item_Name"].ToString(), rows["Item_Code"].ToString(), null, null, true, "Pcs", 1, 15, 1, null, "FINISH");
                            vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { rows["Item_Code"].ToString() });
                            vm = vms.FirstOrDefault();

                        }
                        catch
                        {

                        }
                    }

                    rows["Total_Price"] = (Convert.ToDecimal(rows["Quantity"])) * (vm.Packetprice < 1 ? 1 : vm.Packetprice);
                    rows["VAT_Amount"] = (Convert.ToDecimal(rows["Total_Price"])) * 15 / 100;
                    if (rows["Transaction_Type"].ToString() == "Local Type")
                    {
                        rows["Transaction_Type"] = "Other";
                    }


                    if (finalTable.Columns.Contains("Invoice_Date"))
                    {
                        string pattern = "yyyyMMdd";
                        DateTime parsedDate;
                        var a = rows["Invoice_Date"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["Invoice_Date"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (finalTable.Columns.Contains("Receive_Date"))
                    {
                        string pattern = "yyyyMMdd";
                        DateTime parsedDate;
                        var a = rows["Receive_Date"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["Receive_Date"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (finalTable.Columns.Contains("RebateDate"))
                    {
                        string pattern = "yyyyMMdd";
                        DateTime parsedDate;
                        var a = rows["RebateDate"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["RebateDate"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                finalTable.Columns.Remove("CONVERSION_FACTOR_TO_NOS");
                return finalTable;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable FormatTransferData(DataTable table)
        {
            #region Columns

            Dictionary<string, string> SaleColumns = new Dictionary<string, string>()
            {
                {"ID".ToLower(),"ID"},
                {"Branch_Code".ToLower(),"Branch_Code"},
                {"BranchCode".ToLower(),"BranchCode"},
                {"Transaction_Date".ToLower(),"Transaction_Date"},
                {"Transaction_Time".ToLower(),"Transaction_Time"},
                {"ProductCode".ToLower(),"ProductCode"},
                {"Product_Code".ToLower(),"Product_Code"},
                {"ProductName".ToLower(),"ProductName"},
                {"Product_Name".ToLower(),"Product_Name"},
                {"Post".ToLower(),"Post"},
                {"Comments".ToLower(),"Comments"},
                {"Transaction_Type".ToLower(),"Transaction_Type"},
                {"Quantity".ToLower(),"Quantity"},
                {"CostPrice".ToLower(),"CostPrice"},
                {"Cost_Price".ToLower(),"Cost_Price"},
                {"UOM".ToLower(),"UOM"},
                {"Weight".ToLower(),"Weight"},
                {"ReferenceNo".ToLower(),"ReferenceNo"},
                {"Reference_No".ToLower(),"Reference_No"},
                {"VehicleNo".ToLower(),"VehicleNo"},
                {"Vehicle_No".ToLower(),"Vehicle_No"},
                {"VehicleType".ToLower(),"VehicleType"},
                {"Vehicle_Type".ToLower(),"Vehicle_Type"},
                {"TransactionType".ToLower(),"TransactionType"},
                {"VAT_Rate".ToLower(),"VAT_Rate"},
                {"TransferToBranchCode".ToLower(),"TransferToBranchCode"},
                {"Transfer_To_Branch_Code".ToLower(),"Transfer_To_Branch_Code"},
                {"CONVERSION_FACTOR_TO_NOS".ToLower(),"CONVERSION_FACTOR_TO_NOS"},
          
                
            };

            #endregion
            try
            {
                var finalTable = new DataTable();

                foreach (DataColumn col in table.Columns)
                {
                    var clName = col.ColumnName.ToLower();

                    if (!SaleColumns.Keys.Contains(clName))
                        continue;

                    clName = SaleColumns[clName];

                    //table.Columns[col.ColumnName].ColumnName = clName;

                    finalTable.Columns.Add(clName);

                }

                foreach (DataRow row in table.Rows)
                {
                    var data = row.ItemArray;
                    finalTable.Rows.Add(data);
                }


                ProductDAL dal = new ProductDAL();
                if (!finalTable.Columns.Contains("Weight"))
                {
                    var columnName = new DataColumn("Weight") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }
                foreach (DataRow rows in finalTable.Rows)
                {
                    rows["Weight"] = rows["CONVERSION_FACTOR_TO_NOS"].ToString() == "1" ? "0" : rows["Quantity"].ToString();

                    rows["Reference_No"] = rows["ID"].ToString();
                    if (string.IsNullOrWhiteSpace(rows["CONVERSION_FACTOR_TO_NOS"].ToString()))
                    {
                        rows["UOM"] = "Pcs";
                    }
                    else
                    {
                        rows["UOM"] = "Pcs";
                        rows["Quantity"] = Convert.ToDecimal(rows["Quantity"]) * Convert.ToDecimal(rows["CONVERSION_FACTOR_TO_NOS"]);
                    }
                    rows["Post"] = "N";

                    if (finalTable.Columns.Contains("Transaction_Date"))
                    {
                        string pattern = "dd.MM.yyyy";
                        DateTime parsedDate;
                        var a = rows["Transaction_Date"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["Transaction_Date"] = parsedDate.ToString("yyyy-MM-dd");
                    }

                    if (finalTable.Columns.Contains("Transaction_Time"))
                    {
                        string pattern = "HHmmss";
                        DateTime parsedDate;
                        var a = rows["Transaction_Time"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["Transaction_Time"] = parsedDate.ToString("HH:mm:ss");
                    }

                    string Transaction_Type = rows["Transaction_Type"].ToString();
                    if (Transaction_Type.ToLower() == "finish")
                    {
                        Transaction_Type = "62Out";
                    }
                    else
                    {
                        Transaction_Type = "61Out";
                    }

                    rows["VAT_Rate"] = "0";

                    var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { rows["Product_Code"].ToString() });

                    ProductVM vm = vms.FirstOrDefault();

                    if (vm == null)
                    {
                        try
                        {
                            CommonDAL com = new CommonDAL();
                            CommonImportDAL cImport = new CommonImportDAL();
                            cImport.FindItemId_(rows["Product_Name"].ToString(), rows["Product_Code"].ToString(), null, null, true, "Pcs", 1, 15, 1, null, "FINISH");
                            vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { rows["Product_Code"].ToString() });
                            vm = vms.FirstOrDefault();

                        }
                        catch
                        {

                        }
                    }

                    if (Transaction_Type == "62Out")
                    {
                        rows["Cost_Price"] = (Convert.ToDecimal(rows["Quantity"])) * (vm.NBRPrice < 1 ? 1 : vm.NBRPrice);
                    }
                    else
                    {
                        string NBRPrice = "0";
                        DataTable priceData = dal.AvgPriceNew(vm.ItemNo, rows["Transaction_Date"].ToString() +
                                                DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, true, connVM, Program.CurrentUserID);
                        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                        if (quan > 0)
                        {
                            NBRPrice = (amount / quan).ToString();
                        }
                        else
                        {
                            NBRPrice = "0";
                        }
                        rows["Cost_Price"] = (Convert.ToDecimal(rows["Quantity"])) * Convert.ToDecimal(NBRPrice);
                    }

                    if (string.IsNullOrEmpty(rows["VehicleNo"].ToString()) || rows["VehicleNo"].ToString() == "-")
                    {
                        rows["VehicleNo"] = "NA";
                    }
                    if (string.IsNullOrEmpty(rows["Vehicle_Type"].ToString()) || rows["Vehicle_Type"].ToString() == "-")
                    {
                        rows["Vehicle_Type"] = "NA";
                    }

                }
                finalTable.Columns.Remove("CONVERSION_FACTOR_TO_NOS");

                return finalTable;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void GetFilePurchase(List<string> File)
        {
            CommonDAL _cdal = new CommonDAL();

            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp();

            int loopCounter = 100;

            if (File.Count < loopCounter)
            {
                loopCounter = File.Count;
            }
            for (var index = 0; index < loopCounter; index++)
            {
                string file = PurchaseWorkPath + "/" + File[index];
                string WorkFile = File[index];
                string destinationFile = PurchasePostedPath + "/" + File[index];
                string destinationHoldFile = PurchaseHoldPath + "/" + File[index];
                string importID = "";
                try
                {
                    NewconnVM = Authenticate("RBBH");

                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(PurchaseWorkPath + "/" + File[index]);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.Credentials = new NetworkCredential(User, Pass);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string xml = reader.ReadToEnd();
                    xml = xml.Replace("&", "And");
                    response.Close();

                    DataTable dtpurchase = GetTableFromXML(xml);
                    dtpurchase = FormatPurchaseData(dtpurchase);
                    PurchaseDAL purchaseDal = new PurchaseDAL();
                    dtpurchase.Columns["Branch_Code"].ColumnName = "BranchCode";
                    if (!dtpurchase.Columns.Contains("With_VDS"))
                    {
                        var columnName = new DataColumn("With_VDS") { DefaultValue = "N" };
                        dtpurchase.Columns.Add(columnName);
                    }
                    if (!dtpurchase.Columns.Contains("Rebate_Rate"))
                    {
                        var columnName = new DataColumn("Rebate_Rate") { DefaultValue = 0 };
                        dtpurchase.Columns.Add(columnName);
                    }
                    if (!dtpurchase.Columns.Contains("SD_Amount"))
                    {
                        var columnName = new DataColumn("SD_Amount") { DefaultValue = 0 };
                        dtpurchase.Columns.Add(columnName);
                    }
                    var OtherRef = new DataColumn("OtherRef") { DefaultValue = WorkFile };
                    dtpurchase.Columns.Add(OtherRef);

                    importID = string.Join(",", dtpurchase
                        .DefaultView
                        .ToTable(true, "ID")
                        .Rows.Cast<DataRow>()
                        .Select(x => x["ID"].ToString()));


                    try
                    {
                        if (dtpurchase.Rows.Count > 0)
                        {
                            _cdal.BulkInsert("TempPurchaseMarico", dtpurchase, null, null, 0, null, NewconnVM);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    purchaseDal.SaveTempPurchase(dtpurchase, Program.BranchCode, "Other", Program.CurrentUser, Program.BranchId, () => { }, null, null, NewconnVM, "", false);

                    MoveFile(file, destinationFile, WorkFile, PurchaseLocalStorePath, importID, "Purchase");
                }
                catch (Exception e)
                {
                    try
                    {
                        MoveFile(file, destinationHoldFile, WorkFile, PurchaseLocalStorePath, importID, "Purchase");

                        ErrorLogVM vm = new ErrorLogVM();
                        List<ErrorLogVM> vms = new List<ErrorLogVM>();

                        FileLogger.Log("MARICO", "GetFilePurchase", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                        vm.ImportId = importID;
                        vm.FileName = WorkFile;
                        vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                        vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        vm.SourceName = "MARICO";
                        vm.ActionName = "GetFilePurchase";
                        vm.TransactionName = "Purchase";

                        vms.Add(vm);

                        DataTable dt = vms.ToList().ToDataTable();
                        _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                    }
                    catch
                    {
                    }
                }
            }

        }

        public void GetFileTransfer(List<string> File)
        {
            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp();

            CommonDAL _cdal = new CommonDAL();

            int loopCounter = 100;

            if (File.Count < loopCounter)
            {
                loopCounter = File.Count;
            }
            for (var index = 0; index < loopCounter; index++)
            {
                string file = TransferWorkPath + "/" + File[index];
                string WorkFile = File[index];
                string destinationFile = TransferPostedPath + "/" + File[index];
                string destinationHoldFile = TransferHoldPath + "/" + File[index];
                string importID = "";
                try
                {
                    NewconnVM = Authenticate("RBBH");

                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(TransferWorkPath + "/" + File[index]);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.Credentials = new NetworkCredential(User, Pass);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string xml = reader.ReadToEnd();
                    response.Close();
                    xml = xml.Replace("%", "");
                    xml = xml.Replace("&", "and");
                    DataTable dtTableResult = GetTableFromXML(xml);
                    dtTableResult = FormatTransferData(dtTableResult);
                    TransferIssueDAL TransferIssueDal = new TransferIssueDAL();
                    //dtpurchase.Columns["Branch_Code"].ColumnName = "BranchCode";

                    importID = string.Join(",", dtTableResult
                      .DefaultView
                      .ToTable(true, "ID")
                      .Rows.Cast<DataRow>()
                      .Select(x => x["ID"].ToString()));

                    if (dtTableResult.Columns.Contains("Branch_Code"))
                    {
                        dtTableResult.Columns["Branch_Code"].ColumnName = "BranchCode";
                    }
                    if (dtTableResult.Columns.Contains("Transaction_Type"))
                    {
                        dtTableResult.Columns["Transaction_Type"].ColumnName = "TransactionType";
                    }
                    if (dtTableResult.Columns.Contains("Transaction_Type"))
                    {
                        dtTableResult.Columns["Transaction_Type"].ColumnName = "TransactionType";
                    }
                    if (dtTableResult.Columns.Contains("Product_Code"))
                    {
                        dtTableResult.Columns["Product_Code"].ColumnName = "ProductCode";
                    }
                    if (dtTableResult.Columns.Contains("Product_Name"))
                    {
                        dtTableResult.Columns["Product_Name"].ColumnName = "ProductName";
                    }
                    if (dtTableResult.Columns.Contains("Reference_No"))
                    {
                        dtTableResult.Columns["Reference_No"].ColumnName = "ReferenceNo";
                    }
                    if (dtTableResult.Columns.Contains("Transfer_To_Branch_Code"))
                    {
                        dtTableResult.Columns["Transfer_To_Branch_Code"].ColumnName = "TransferToBranchCode";
                    }
                    if (dtTableResult.Columns.Contains("Vehicle_Type"))
                    {
                        dtTableResult.Columns["Vehicle_Type"].ColumnName = "VehicleType";
                    }
                    if (dtTableResult.Columns.Contains("Vehicle_No"))
                    {
                        dtTableResult.Columns["Vehicle_No"].ColumnName = "VehicleNo";
                    }

                    if (dtTableResult.Columns.Contains("Cost_Price"))
                    {
                        dtTableResult.Columns["Cost_Price"].ColumnName = "CostPrice";
                    }

                    string Transaction_Type = "62Out";
                    if (dtTableResult.Rows.Count > 0)
                    {
                        Transaction_Type = dtTableResult.Rows[0]["TransactionType"].ToString();
                        if (Transaction_Type.ToLower() == "finish")
                        {
                            Transaction_Type = "62Out";
                        }
                        else
                        {
                            Transaction_Type = "61Out";
                        }

                    }
                    try
                    {
                        if (dtTableResult.Rows.Count > 0)
                        {
                            _cdal.BulkInsert("TempTransferMarico", dtTableResult, null, null, 0, null, NewconnVM);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    if (!dtTableResult.Columns.Contains("OtherRef"))
                    {
                        var columnName = new DataColumn("OtherRef") { DefaultValue = File[index] };
                        dtTableResult.Columns.Add(columnName);
                    }
                    TransferIssueDal.SaveTempTransfer(dtTableResult, Program.BranchCode, Transaction_Type,
                    Program.CurrentUser, Program.BranchId, () => { }, null, null, false, NewconnVM, "", Program.CurrentUserID);

                    MoveFile(file, destinationFile, WorkFile, TransferLocalStorePath, importID, "Transfer");
                }
                catch (Exception e)
                {
                    try
                    {
                        MoveFile(file, destinationHoldFile, WorkFile, TransferLocalStorePath, importID, "Transfer");

                        ErrorLogVM vm = new ErrorLogVM();
                        List<ErrorLogVM> vms = new List<ErrorLogVM>();


                        FileLogger.Log("MARICO", "GetFileTransfer", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                        vm.ImportId = importID;
                        vm.FileName = WorkFile;
                        vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                        vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        vm.SourceName = "MARICO";
                        vm.ActionName = "GetFileTransfer";
                        vm.TransactionName = "Transfer";

                        vms.Add(vm);

                        DataTable dt = vms.ToList().ToDataTable();
                        _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                    }
                    catch
                    {

                    }
                }
            }

        }

        private DataTable GetTableFromXML(string xml)
        {
            #region Variables
            DataTable dt = new DataTable();

            #endregion
            DataSet dsResult = OrdinaryVATDesktop.GetDataSetFromXML(xml);

            if (dsResult.Tables.Count > 0)
            {
                dt = dsResult.Tables[0];
            }
            return dt;
        }

        private void MoveFile(string sourceFile, string destinationFile, string WorkFile = null, string LocalStore = null, string importID = null, string TransactionName = null)
        {
            try
            {
                #region DownLoad File Restore to local
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sourceFile);

                request.Credentials = new NetworkCredential(User, Pass);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                using (Stream fileStream = File.Create(LocalStore + WorkFile))
                {
                    ftpStream.CopyTo(fileStream);
                }
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                #endregion

                #region Upload File local to Remote

                //            FtpWebRequest requestUpload =
                //(FtpWebRequest)WebRequest.Create(destinationFile);
                //            requestUpload.Credentials = new NetworkCredential(User, Pass);
                //            requestUpload.Method = WebRequestMethods.Ftp.UploadFile;

                //            using (Stream fileStream = File.OpenRead(LocalStore + WorkFile))
                //            using (Stream ftpStream = requestUpload.GetRequestStream())
                //            {
                //                fileStream.CopyTo(ftpStream);
                //            }
                //            FtpWebResponse responseUpload = (FtpWebResponse)requestUpload.GetResponse();
                //            responseUpload.Close();

                Directory.Move(LocalStore + WorkFile, destinationFile);

                #endregion

                #region Delete  File From Remote

                FtpWebRequest requesRemote = (FtpWebRequest)WebRequest.Create(sourceFile);
                requesRemote.Method = WebRequestMethods.Ftp.DeleteFile;
                requesRemote.Credentials = new NetworkCredential(User, Pass);
                FtpWebResponse responseRemote = (FtpWebResponse)requesRemote.GetResponse();
                responseRemote.Close();
                #endregion

                File.Delete(LocalStore + WorkFile);

            }
            catch (Exception e)
            {
                try
                {

                    CommonDAL _cdal = new CommonDAL();

                    ErrorLogVM vm = new ErrorLogVM();
                    List<ErrorLogVM> vms = new List<ErrorLogVM>();
                    FileLogger.Log("MARICO", "MoveFile", e.Message + "\n" + e.StackTrace);

                    vm.ImportId = importID;
                    vm.FileName = WorkFile;
                    vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                    vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    vm.SourceName = "MARICO";
                    vm.ActionName = "MoveFile";
                    vm.TransactionName = TransactionName;

                    vms.Add(vm);

                    DataTable dt = vms.ToList().ToDataTable();
                    _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, connVM);
                }
                catch
                {

                }

            }
        }

        private FtpWebRequest CreateFtpWebRequest(string ftpDirectoryPath, string userName, string password, bool keepAlive = false)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpDirectoryPath));

            //Set proxy to null. Under current configuration if this option is not set then the proxy that is used will get an html response from the web content gateway (firewall monitoring system)
            request.Proxy = null;

            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = keepAlive;

            request.Credentials = new NetworkCredential(userName, password);

            return request;
        }
        private void DownloadFile(string userName, string password, string ftpSourceFilePath, string localDestinationFilePath)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[2048];

            FtpWebRequest request = CreateFtpWebRequest(ftpSourceFilePath, userName, password, true);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            Stream reader = request.GetResponse().GetResponseStream();
            FileStream fileStream = new FileStream(localDestinationFilePath, FileMode.Create);

            while (true)
            {
                bytesRead = reader.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                    break;

                fileStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
        }
        private void TableValidation(DataTable salesData, string fileName = "")
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
            var OtherRef = new DataColumn("OtherRef") { DefaultValue = fileName };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);
            salesData.Columns.Add(OtherRef);

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

        private void bgwSaveUnprocessed_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //_timePassedInMs = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private bool isProcessRunning;

        #region For Marico Path

        ////string SaleWorkPath = @"";
        ////string SaleLocal = @"F:\Sales";
        ////string SaleHoldPath = @"F:\Sales\Hold";
        ////string SalePostedPath = @"F:\Sales\Posted";
        ////string SaleLocalStorePath = @"F:\Sales\Work\";

        ////string PurchaseWorkPath = @"";
        ////string Purchase = @"F:\Purchases";
        ////string PurchaseHoldPath = @"F:\Purchases\Hold";
        ////string PurchasePostedPath = @"F:\Purchases\Posted";
        ////string PurchaseLocalStorePath = @"F:\Purchases\Work\";

        ////string TransferWorkPath = @"";
        ////string Transfer = @"F:\Transfers";
        ////string TransferHoldPath = @"F:\Transfers\Hold\";
        ////string TransferPostedPath = @"F:\Transfers\Posted";
        ////string TransferLocalStorePath = @"F:\Transfers\Work\";

        ////string FactorySaleWorkPath = @"";
        ////string FactorySaleLocal = @"F:\FactorySales";
        ////string FactorySaleHoldPath = @"F:\FactorySales\Hold";
        ////string FactorySalePostedPath = @"F:\FactorySales\Posted";
        ////string FactorySaleLocalStorePath = @"F:\FactorySales\Work\";

        ////string FactoryTransferWorkPath = @"";
        ////string FactoryTransfer = @"F:\FactoryTransfers";
        ////string FactoryTransferHoldPath = @"F:\FactoryTransfers\Hold\";
        ////string FactoryTransferPostedPath = @"F:\FactoryTransfers\Posted";
        ////string FactoryTransferLocalStorePath = @"F:\FactoryTransfers\Work\";

        ////string ContractManufacturingWorkPath = @"";
        ////string ContractManufacturing = @"F:\ContractManufacturing";
        ////string ContractManufacturingHoldPath = @"F:\ContractManufacturing\Hold\";
        ////string ContractManufacturingPostedPath = @"F:\ContractManufacturing\Posted";
        ////string ContractManufacturingStorePath = @"F:\ContractManufacturing\Work\";

        ////string FactoryPurchaseWorkPath = @"";
        ////string FactoryPurchase = @"F:\FactoryPurchases";
        ////string FactoryPurchaseHoldPath = @"F:\FactoryPurchases\Hold";
        ////string FactoryPurchasePostedPath = @"F:\FactoryPurchases\Posted";
        ////string FactoryPurchaseLocalStorePath = @"F:\FactoryPurchases\Work\";

        ////string FactoryIssueWorkPath = @"";
        ////string FactoryIssue = @"F:\FactoryIssue";
        ////string FactoryIssueHoldPath = @"F:\FactoryIssue\Hold";
        ////string FactoryIssuePostedPath = @"F:\FactoryIssue\Posted";
        ////string FactoryIssueLocalStorePath = @"F:\FactoryIssue\Work\";


        #endregion

        #region For Symphony path

        //string SaleWorkPath = @"";
        //string SaleLocal = @"E:\Sales";
        //string SaleHoldPath = @"E:\Sales\Hold";
        //string SalePostedPath = @"E:\Sales\Posted";
        //string SaleLocalStorePath = @"E:\Sales\Work\";

        //string PurchaseWorkPath = @"";
        //string Purchase = @"E:\Purchases";
        //string PurchaseHoldPath = @"E:\Purchases\Hold";
        //string PurchasePostedPath = @"E:\Purchases\Posted";
        //string PurchaseLocalStorePath = @"E:\Purchases\Work\";

        //string TransferWorkPath = @"";
        //string Transfer = @"E:\Transfers";
        //string TransferHoldPath = @"E:\Transfers\Hold\";
        //string TransferPostedPath = @"E:\Transfers\Posted";
        //string TransferLocalStorePath = @"E:\Transfers\Work\";

        //string FactorySaleWorkPath = @"";
        //string FactorySaleLocal = @"E:\FactorySales";
        //string FactorySaleHoldPath = @"E:\FactorySales\Hold";
        //string FactorySalePostedPath = @"E:\FactorySales\Posted";
        //string FactorySaleLocalStorePath = @"E:\FactorySales\Work\";

        //string FactoryTransferWorkPath = @"";
        //string FactoryTransfer = @"E:\FactoryTransfers";
        //string FactoryTransferHoldPath = @"E:\FactoryTransfers\Hold\";
        //string FactoryTransferPostedPath = @"E:\FactoryTransfers\Posted";
        //string FactoryTransferLocalStorePath = @"E:\FactoryTransfers\Work\";

        //string ContractManufacturingWorkPath = @"";
        //string ContractManufacturing = @"E:\ContractManufacturing";
        //string ContractManufacturingHoldPath = @"E:\ContractManufacturing\Hold\";
        //string ContractManufacturingPostedPath = @"E:\ContractManufacturing\Posted";
        //string ContractManufacturingStorePath = @"E:\ContractManufacturing\Work\";

        //string FactoryPurchaseWorkPath = @"";
        //string FactoryPurchase = @"E:\FactoryPurchases";
        //string FactoryPurchaseHoldPath = @"E:\FactoryPurchases\Hold";
        //string FactoryPurchasePostedPath = @"E:\FactoryPurchases\Posted";
        //string FactoryPurchaseLocalStorePath = @"E:\FactoryPurchases\Work\";

        //string FactoryIssueWorkPath = @"";
        //string FactoryIssue = @"E:\FactoryIssue";
        //string FactoryIssueHoldPath = @"E:\FactoryIssue\Hold";
        //string FactoryIssuePostedPath = @"E:\FactoryIssue\Posted";
        //string FactoryIssueLocalStorePath = @"E:\FactoryIssue\Work\";


        #endregion

        #region FilePath

        string SaleWorkPath = @"";
        string SaleLocal = @"F:\Sales";
        string SaleHoldPath = @"F:\Sales\Hold";
        string SalePostedPath = @"F:\Sales\Posted";
        string SaleLocalStorePath = @"F:\Sales\Work\";

        string PurchaseWorkPath = @"";
        string Purchase = @"F:\Purchases";
        string PurchaseHoldPath = @"F:\Purchases\Hold";
        string PurchasePostedPath = @"F:\Purchases\Posted";
        string PurchaseLocalStorePath = @"F:\Purchases\Work\";

        string TransferWorkPath = @"";
        string Transfer = @"F:\Transfers";
        string TransferHoldPath = @"F:\Transfers\Hold\";
        string TransferPostedPath = @"F:\Transfers\Posted";
        string TransferLocalStorePath = @"F:\Transfers\Work\";

        string FactorySaleWorkPath = @"";
        string FactorySaleLocal = @"F:\FactorySales";
        string FactorySaleHoldPath = @"F:\FactorySales\Hold";
        string FactorySalePostedPath = @"F:\FactorySales\Posted";
        string FactorySaleLocalStorePath = @"F:\FactorySales\Work\";

        string FactoryTransferWorkPath = @"";
        string FactoryTransfer = @"F:\FactoryTransfers";
        string FactoryTransferHoldPath = @"F:\FactoryTransfers\Hold\";
        string FactoryTransferPostedPath = @"F:\FactoryTransfers\Posted";
        string FactoryTransferLocalStorePath = @"F:\FactoryTransfers\Work\";

        string ContractManufacturingWorkPath = @"";
        string ContractManufacturing = @"F:\ContractManufacturing";
        string ContractManufacturingHoldPath = @"F:\ContractManufacturing\Hold\";
        string ContractManufacturingPostedPath = @"F:\ContractManufacturing\Posted";
        string ContractManufacturingStorePath = @"F:\ContractManufacturing\Work\";

        string FactoryPurchaseWorkPath = @"";
        string FactoryPurchase = @"F:\FactoryPurchases";
        string FactoryPurchaseHoldPath = @"F:\FactoryPurchases\Hold";
        string FactoryPurchasePostedPath = @"F:\FactoryPurchases\Posted";
        string FactoryPurchaseLocalStorePath = @"F:\FactoryPurchases\Work\";

        string FactoryIssueWorkPath = @"";
        string FactoryIssue = @"F:\FactoryIssue";
        string FactoryIssueHoldPath = @"F:\FactoryIssue\Hold";
        string FactoryIssuePostedPath = @"F:\FactoryIssue\Posted";
        string FactoryIssueLocalStorePath = @"F:\FactoryIssue\Work\";


        #endregion

        string User = "VATClient";
        string Pass = "S123456_";

        ////////For Live
        //private string User = "mblevatprd";//"MBLEVATUAT";
        //string Pass = "Jfk34#@2022";

        ////////For test
        ////private string User = "MBLEVATUAT";
        ////string Pass = "Jfk34#@2022";

        private void btnBulk_Click(object sender, EventArgs e)
        {

            try
            {
                var value = new CommonDAL().settingsDesktop("CompanyCode", "Code", null, connVM);

                if (!isSymphony)
                {
                    if (value.ToLower() != "mbl")
                    {
                        MessageBox.Show("Please goto Marico Bangladesh Ltd (CSD) and then turn on the process");
                        return;
                    }
                }

                if (isSymphony)
                {

                    #region For Symphony test

                    ////SaleWorkPath = @"ftp://103.231.239.122/Sales/Work";
                    ////PurchaseWorkPath = @"ftp://103.231.239.122/Purchase/Work";
                    ////TransferWorkPath = @"ftp://103.231.239.122/Internal Order/Work";

                    ////FactorySaleWorkPath = @"ftp://103.231.239.122/Factory Sales/Work";
                    ////FactoryTransferWorkPath = @"ftp://103.231.239.122/Internal Transfer/Work";
                    ////ContractManufacturingWorkPath = @"ftp://103.231.239.122/Contract Manufacturing/Work";
                    ////FactoryPurchaseWorkPath = @"ftp://103.231.239.122/Purchase Data/Work";
                    ////FactoryIssueWorkPath = @"ftp://103.231.239.122/Issue and Consumption/Work";

                    SaleWorkPath = @"ftp://192.168.15.100:22/Sales/Work";
                    PurchaseWorkPath = @"ftp://192.168.15.100:22/Purchase/Work";
                    TransferWorkPath = @"ftp://192.168.15.100:22/Internal Order/Work";

                    FactorySaleWorkPath = @"ftp://192.168.15.100:22/Factory Sales/Work";
                    FactoryTransferWorkPath = @"ftp://192.168.15.100:22/Internal Transfer/Work";
                    ContractManufacturingWorkPath = @"ftp://192.168.15.100:22/Contract Manufacturing/Work";
                    FactoryPurchaseWorkPath = @"ftp://192.168.15.100:22/Purchase Data/Work";
                    FactoryIssueWorkPath = @"ftp://192.168.15.100:22/Issue and Consumption/Work";


                    #endregion
                }
                else
                {

                    SaleWorkPath = @"ftp://ftp2.h2h.biz:21/Sales/Work";
                    PurchaseWorkPath = @"ftp://ftp2.h2h.biz:21/Purchase/Work";
                    TransferWorkPath = @"ftp://ftp2.h2h.biz:21/Internal Order/Work";

                    FactorySaleWorkPath = @"ftp://ftp2.h2h.biz:21/Factory Sales/Work";
                    FactoryTransferWorkPath = @"ftp://ftp2.h2h.biz:21/Internal Transfer/Work";
                    ContractManufacturingWorkPath = @"ftp://ftp2.h2h.biz:21/Contract Manufacturing/Work";
                    FactoryPurchaseWorkPath = @"ftp://ftp2.h2h.biz:21/Purchase Data/Work";
                    FactoryIssueWorkPath = @"ftp://ftp2.h2h.biz:21/Issue and Consumption/Work";

                }


                Directory.CreateDirectory(SaleLocal + "\\Work");
                Directory.CreateDirectory(Purchase + "\\Work");
                Directory.CreateDirectory(Transfer + "\\Work");

                Directory.CreateDirectory(FactorySaleLocal + "\\Work");
                Directory.CreateDirectory(FactoryTransfer + "\\Work");
                Directory.CreateDirectory(ContractManufacturing + "\\Work");
                Directory.CreateDirectory(FactoryPurchase + "\\Work");
                Directory.CreateDirectory(FactoryIssue + "\\Work");

                Directory.CreateDirectory(SaleLocal + "\\Hold");
                Directory.CreateDirectory(Purchase + "\\Hold");
                Directory.CreateDirectory(Transfer + "\\Hold");

                Directory.CreateDirectory(FactorySaleLocal + "\\Hold");
                Directory.CreateDirectory(FactoryTransfer + "\\Hold");
                Directory.CreateDirectory(ContractManufacturing + "\\Hold");
                Directory.CreateDirectory(FactoryPurchase + "\\Hold");
                Directory.CreateDirectory(FactoryIssue + "\\Hold");

                Directory.CreateDirectory(SaleLocal + "\\Posted");
                Directory.CreateDirectory(Purchase + "\\Posted");
                Directory.CreateDirectory(Transfer + "\\Posted");

                Directory.CreateDirectory(FactorySaleLocal + "\\Posted");
                Directory.CreateDirectory(FactoryTransfer + "\\Posted");
                Directory.CreateDirectory(ContractManufacturing + "\\Posted");
                Directory.CreateDirectory(FactoryPurchase + "\\Posted");
                Directory.CreateDirectory(FactoryIssue + "\\Posted");

                if (!timer1.Enabled)
                {
                    timer1.Start();
                }

                FileLogger.Log("btnBulk_Click", "btnBulk_Click", "Start");

                MessageBox.Show("Process Started");

            }
            catch (Exception exception)
            {
                FileLogger.Log("Start FIle Watcher", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);

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

        private void bgwMARICO_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetFile(SaleFile);
            }
            catch (Exception exception)
            {
                FileLogger.Log("bgwMARICO", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void bgwMARICO_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isProcessRunning = false;
            progressBar1.Visible = false;

            //RunIAS();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                FileLogger.Log("timer1_Tick", "timer1_Tick", isProcessRunning.ToString());

                if (!isProcessRunning)
                {

                    SaleFile = FileCount(SaleWorkPath);
                    PurchaseFile = FileCount(PurchaseWorkPath);
                    TransferFile = FileCount(TransferWorkPath);

                    FactorySaleFile = FileCount(FactorySaleWorkPath);
                    FactoryTransferFile = FileCount(FactoryTransferWorkPath);
                    FactoryContractManufacturingFile = FileCount(ContractManufacturingWorkPath);
                    FactoryPurchaseFile = FileCount(FactoryPurchaseWorkPath);
                    FactoryIssueFile = FileCount(FactoryIssueWorkPath);


                    FileLogger.Log("SaleFile", "SaleFile", SaleFile.ToString());
                    FileLogger.Log("PurchaseFile", "PurchaseFile", PurchaseFile.ToString());
                    FileLogger.Log("TransferFile", "TransferFile", TransferFile.ToString());

                    if (SaleFile.Count > 0)
                    {
                        progressBar1.Visible = true;
                        isProcessRunning = true;
                        bgwMARICO.RunWorkerAsync();
                    }
                    else if (FactorySaleFile.Count > 0)
                    {
                        progressBar1.Visible = true;
                        isProcessRunning = true;
                        bgwSaveSales.RunWorkerAsync();
                    }
                    else if (TransferFile.Count > 0)
                    {
                        progressBar1.Visible = true;
                        isProcessRunning = true;
                        bgwMaricoTransfer.RunWorkerAsync();
                    }
                    else if (FactoryTransferFile.Count > 0)
                    {
                        progressBar1.Visible = true;
                        isProcessRunning = true;
                        bgwMaricoFactoryTransfer.RunWorkerAsync();
                    }
                    else if (FactoryContractManufacturingFile.Count > 0)
                    {
                        progressBar1.Visible = true;
                        isProcessRunning = true;
                        bgwMaricoContractManufacturing.RunWorkerAsync();
                    }
                    else if (PurchaseFile.Count > 0)
                    {
                        progressBar1.Visible = true;
                        isProcessRunning = true;
                        bgwMARICOPurchase.RunWorkerAsync();
                    }
                    else if (FactoryPurchaseFile.Count > 0)
                    {
                        progressBar1.Visible = true;
                        isProcessRunning = true;
                        bgwMARICOFactoryPurchase.RunWorkerAsync();
                    }
                    else if (FactoryIssueFile.Count > 0)
                    {
                        progressBar1.Visible = true;
                        isProcessRunning = true;
                        bgwMARICOFactoryIssue.RunWorkerAsync();
                    }

                }

            }
            catch (Exception exception)
            {
                FileLogger.Log("timer1_Tick", "timer1_Tick", exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void bgwMARICOPurchase_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                GetFilePurchase(PurchaseFile);
            }
            catch (Exception exception)
            {
                FileLogger.Log("bgwMARICOPurchase", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);
            }

        }

        private void bgwMARICOPurchase_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isProcessRunning = false;
            progressBar1.Visible = false;
        }

        private List<string> FileCount(string Path = "")
        {

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                request.Credentials = new NetworkCredential(User, Pass);
                request.KeepAlive = false;
                request.UseBinary = true;
                request.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                List<string> entries = new List<string>();
                List<string> File = new List<string>();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    entries = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                foreach (string entry in entries)
                {
                    string[] splits = entry.Split(new string[] { " ", }, StringSplitOptions.RemoveEmptyEntries);
                    File.Add(splits[3]);

                }
                response.Close();

                return File;
            }
            catch (Exception exception)
            {
                FileLogger.Log("btnBulk_Click", "FileCount", exception.Message + "\n" + exception.StackTrace);
                return new List<string> { };
            }

        }

        private void bgwMaricoTransfer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetFileTransfer(TransferFile);
            }
            catch (Exception exception)
            {
                FileLogger.Log("bgwMaricoTransfer", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);
            }

        }

        private void bgwMaricoTransfer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            isProcessRunning = false;
            progressBar1.Visible = false;
        }

        private void bgwSaveSales_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetFileSalesFactory(FactorySaleFile);
            }
            catch (Exception exception)
            {
                FileLogger.Log("SalesImportMARICO", "bgwSaveSales_DoWork", exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void bgwSaveSales_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isProcessRunning = false;
            progressBar1.Visible = false;

        }

        public void GetFileSalesFactory(List<string> File)
        {
            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp();

            int loopCounter = 100;

            if (File.Count < loopCounter)
            {
                loopCounter = File.Count;
            }
            for (var index = 0; index < loopCounter; index++)
            {
                string file = FactorySaleWorkPath + "/" + File[index];
                string WorkFile = File[index];
                string destinationFile = FactorySalePostedPath + "/" + File[index];
                string destinationHoldFile = FactorySaleHoldPath + "/" + File[index];
                string importID = "";
                DataTable finaldata = new DataTable();
                try
                {
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FactorySaleWorkPath + "/" + File[index]);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.Credentials = new NetworkCredential(User, Pass);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string xml = reader.ReadToEnd();
                    response.Close();
                    xml = xml.Replace("&", "And");

                    DataTable dtsale = GetTableFromXML(xml);

                    dtsale = FormatSaleFactoryData(dtsale);

                    TableValidation(dtsale, File[index]);

                    SaleDAL saleDal = new SaleDAL();

                    #region Mouchak

                    DataRow[] rows = dtsale.Select("Branch_Code = 'PB23'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();
                            NewconnVM = Authenticate("PB23");

                            importID = string.Join(",", finaldata
                               .DefaultView
                               .ToTable(true, "ID")
                               .Rows.Cast<DataRow>()
                               .Select(x => x["ID"].ToString()));

                            saleDal.SaveAndProcess(finaldata, () => { }, Program.BranchId, "", NewconnVM);
                        }
                        catch (Exception e)
                        {
                            CommonDAL _cdal = new CommonDAL();

                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO", "GetFileSalesFactory", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB23";
                            vm.ActionName = "GetFileSalesFactory";
                            vm.TransactionName = "SaleFactory";
                            vms.Add(vm);

                            DataTable dt = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);

                        }

                    }

                    #endregion

                    #region Shirirchala

                    DataRow[] dtrows = dtsale.Select("Branch_Code = 'PBBO'");

                    if (dtrows.Count() > 0)
                    {
                        finaldata = new DataTable();

                        finaldata = dtrows.CopyToDataTable();
                        NewconnVM = Authenticate("PBBO");

                        importID = string.Join(",", finaldata
                       .DefaultView
                       .ToTable(true, "ID")
                       .Rows.Cast<DataRow>()
                       .Select(x => x["ID"].ToString()));

                        saleDal.SaveAndProcess(finaldata, () => { }, Program.BranchId, "", NewconnVM);

                    }

                    #endregion

                    #region Mirsarai

                    rows = dtsale.Select("Branch_Code = 'PB81'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();
                            NewconnVM = Authenticate("PB81");

                            importID = string.Join(",", finaldata
                                .DefaultView
                                .ToTable(true, "ID")
                                .Rows.Cast<DataRow>()
                                .Select(x => x["ID"].ToString()));

                            saleDal.SaveAndProcess(finaldata, () => { }, Program.BranchId, "", NewconnVM);
                        }
                        catch (Exception e)
                        {
                            CommonDAL _cdal = new CommonDAL();

                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO", "GetFileSalesFactory", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB81";
                            vm.ActionName = "GetFileSalesFactory";
                            vm.TransactionName = "SaleFactory";
                            vms.Add(vm);

                            DataTable dt = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);

                        }

                    }

                    #endregion

                    MoveFile(file, destinationFile, WorkFile, FactorySaleLocalStorePath, importID, "SaleFactory");

                }
                catch (Exception e)
                {
                    try
                    {

                        CommonDAL _cdal = new CommonDAL();
                        MoveFile(file, destinationHoldFile, WorkFile, FactorySaleLocalStorePath, importID, "SaleFactory");

                        ErrorLogVM vm = new ErrorLogVM();
                        List<ErrorLogVM> vms = new List<ErrorLogVM>();

                        FileLogger.Log("MARICO", "GetFileSalesFactory", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                        vm.ImportId = importID;
                        vm.FileName = WorkFile;
                        vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                        vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        vm.SourceName = "MARICO";
                        vm.ActionName = "GetFileSalesFactory";
                        vm.TransactionName = "SaleFactory";
                        vms.Add(vm);

                        DataTable dt = vms.ToList().ToDataTable();
                        _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                    }
                    catch
                    {

                    }
                }
            }

        }

        public static DataTable FormatSaleFactoryData(DataTable table)
        {

            try
            {
                var finalTable = new DataTable();

                finalTable = table.Copy();

                #region Column Name Change

                ////finalTable.Columns["PREV_INV_DATE_TIME"].ColumnName = "PreviousInvoiceDateTime";
                finalTable.Columns["PLANT_CODE"].ColumnName = "Branch_Code";
                finalTable.Columns["CUSTOMER_CODE"].ColumnName = "Customer_Code";
                finalTable.Columns["CUSTOMER_NAME"].ColumnName = "Customer_Name";
                finalTable.Columns["REFERENCE_NO"].ColumnName = "Reference_No";

                finalTable.Columns["ITEM_CODE"].ColumnName = "Item_Code";
                finalTable.Columns["ITEM_NAME"].ColumnName = "Item_Name";
                finalTable.Columns["QUANTITY"].ColumnName = "Quantity";
                finalTable.Columns["NBR_PRICE"].ColumnName = "NBR_Price";
                finalTable.Columns["WEIGHT"].ColumnName = "Weight";
                finalTable.Columns["SUB_TOTAL"].ColumnName = "SubTotal";
                finalTable.Columns["VEHICLE_NO"].ColumnName = "Vehicle_No";
                finalTable.Columns["VEHICLE_TYPE"].ColumnName = "VehicleType";
                finalTable.Columns["CUSTOMER_GROUP"].ColumnName = "CustomerGroup";
                finalTable.Columns["DELIVERY_ADDRESS"].ColumnName = "Delivery_Address";
                finalTable.Columns["COMMENTS"].ColumnName = "Comments";
                finalTable.Columns["SALE_TYPE"].ColumnName = "Sale_Type";
                finalTable.Columns["TRANSACTION_TYPE"].ColumnName = "TransactionType";
                finalTable.Columns["IS_PRINT"].ColumnName = "Is_Print";
                finalTable.Columns["TENDER_ID"].ColumnName = "Tender_Id";
                finalTable.Columns["POST"].ColumnName = "Post";
                finalTable.Columns["LC_NUMBER"].ColumnName = "LC_Number";
                finalTable.Columns["CURRENCY"].ColumnName = "Currency_Code";
                finalTable.Columns["COMMENTSD"].ColumnName = "CommentsD";
                finalTable.Columns["SD_RATE"].ColumnName = "SD_Rate";
                finalTable.Columns["SD_AMOUNT"].ColumnName = "SDAmount";
                finalTable.Columns["VAT_RATE"].ColumnName = "VAT_Rate";
                finalTable.Columns["VAT_AMOUNT"].ColumnName = "VAT_Amount";
                finalTable.Columns["NON_STOCK"].ColumnName = "Non_Stock";
                finalTable.Columns["TRADING_MARKUP"].ColumnName = "Trading_MarkUp";
                finalTable.Columns["TYPE"].ColumnName = "Type";
                finalTable.Columns["DISC_AMOUNT"].ColumnName = "Discount_Amount";
                finalTable.Columns["PROMO_QUANTITY"].ColumnName = "Promotional_Quantity";
                finalTable.Columns["VAT_NAME"].ColumnName = "VAT_Name";
                finalTable.Columns["PRODUCT_DESCRIPTION"].ColumnName = "ProductDescription";
                finalTable.Columns["PREV_INVOICE"].ColumnName = "Previous_Invoice_No";
                finalTable.Columns["PREV_NBR"].ColumnName = "PreviousNBRPrice";
                finalTable.Columns["PREV_QUANTITY"].ColumnName = "PreviousQuantity";
                finalTable.Columns["PREV_SUBTOTAL"].ColumnName = "PreviousSubTotal";
                finalTable.Columns["PREV_SD"].ColumnName = "PreviousSD";
                finalTable.Columns["PREV_SD_AMOUNT"].ColumnName = "PreviousSDAmount";
                finalTable.Columns["PREV_VAT_RATE"].ColumnName = "PreviousVATRate";
                finalTable.Columns["PREV_VAT_AMOUNT"].ColumnName = "PreviousVATAmount";
                finalTable.Columns["PREV_UOM"].ColumnName = "PreviousUOM";
                finalTable.Columns["RETURN_REASON"].ColumnName = "ReasonOfReturn";

                #endregion

                if (!finalTable.Columns.Contains("Invoice_Date_Time"))
                {
                    var columnName = new DataColumn("Invoice_Date_Time") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }

                if (!finalTable.Columns.Contains("PreviousNBRPrice"))
                {
                    var columnName = new DataColumn("PreviousNBRPrice") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("PreviousInvoiceDateTime"))
                {
                    var columnName = new DataColumn("PreviousInvoiceDateTime") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("CustomerBIN"))
                {
                    var columnName = new DataColumn("CustomerBIN") { DefaultValue = "XNAX" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Weight"))
                {
                    var columnName = new DataColumn("Weight") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }
                foreach (DataRow rows in finalTable.Rows)
                {

                    ////if (rows["Item_Code"].ToString() == "301111" || rows["Item_Code"].ToString() == "300804")
                    ////{
                    ////    rows["UOM"] = "L";
                    ////    rows["CONVERSION_FACTOR"] = "1.0989";
                    ////}

                    ////rows["UOM"] = "Pcs";
                    rows["Weight"] = rows["CONVERSION_FACTOR"].ToString() == "1" ? "0" : rows["Quantity"].ToString();

                    if (!string.IsNullOrWhiteSpace(rows["CONVERSION_FACTOR"].ToString()) && rows["UOM"].ToString().ToLower() != "nos")
                    {
                        rows["Quantity"] = Convert.ToDecimal(rows["Quantity"]) * Convert.ToDecimal(rows["CONVERSION_FACTOR"]);
                        //////rows["NBR_Price"] = Convert.ToDecimal(rows["NBR_Price"]) / Convert.ToDecimal(rows["CONVERSION_FACTOR"]);
                    }

                    if (rows["Sale_Type"].ToString() == "ZBRE")
                    {
                        rows["PreviousUOM"] = "Pcs";

                        rows["PreviousNBRPrice"] = Convert.ToDecimal(rows["PreviousSubTotal"]) / (Convert.ToDecimal(rows["PreviousQuantity"]) == 0 ? 1 : Convert.ToDecimal(rows["PreviousQuantity"]));


                        if (!string.IsNullOrWhiteSpace(rows["PREVIOUS_CONVERSION_FACTOR_TO_NOS"].ToString()) && rows["PreviousUOM"].ToString().ToLower() != "nos")
                        {
                            if (rows["PREVIOUS_CONVERSION_FACTOR_TO_NOS"].ToString() == "0")
                            {
                                rows["PreviousQuantity"] = Convert.ToDecimal(rows["PreviousQuantity"]) * Convert.ToDecimal(rows["CONVERSION_FACTOR_TO_NOS"]);
                                //////rows["PreviousNBRPrice"] = Convert.ToDecimal(rows["PreviousNBRPrice"]) / Convert.ToDecimal(rows["CONVERSION_FACTOR_TO_NOS"]);
                            }
                            else
                            {
                                rows["PreviousQuantity"] = Convert.ToDecimal(rows["PreviousQuantity"]) * Convert.ToDecimal(rows["PREVIOUS_CONVERSION_FACTOR_TO_NOS"]);
                                ////////rows["PreviousNBRPrice"] = Convert.ToDecimal(rows["PreviousNBRPrice"]) / Convert.ToDecimal(rows["PREVIOUS_CONVERSION_FACTOR_TO_NOS"]);
                            }
                        }
                    }

                    if (finalTable.Columns.Contains("PREV_INV_DATE_TIME"))
                    {
                        if (!string.IsNullOrWhiteSpace(rows["PREV_INV_DATE_TIME"].ToString()) && rows["PREV_INV_DATE_TIME"].ToString() != "0")
                        {
                            if (finalTable.Columns.Contains("PreviousInvoiceDateTime"))
                            {
                                string pattern = "dd.MM.yyyy HH:mm:ss";
                                DateTime parsedDate;
                                var a = rows["PREV_INV_DATE_TIME"].ToString();
                                DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                                rows["PreviousInvoiceDateTime"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            if (rows["PreviousInvoiceDateTime"].ToString() == "0001-01-01 00:00:00")
                            {
                                rows["PreviousInvoiceDateTime"] = "1900-01-01 00:00:00";
                            }
                        }

                    }

                    if (string.IsNullOrWhiteSpace(rows["PreviousInvoiceDateTime"].ToString()) || rows["PreviousInvoiceDateTime"].ToString() == "0")
                    {
                        rows["PreviousInvoiceDateTime"] = "1900-01-01 00:00:00";

                    }

                    if (rows["Sale_Type"].ToString() == "ZDLB" || rows["Sale_Type"].ToString() == "ZBOR")
                    {
                        rows["Sale_Type"] = "New";
                        rows["TransactionType"] = "Other";
                    }
                    else if (rows["Sale_Type"].ToString() == "ZBRE")
                    {
                        rows["Sale_Type"] = "Credit";
                        rows["TransactionType"] = "Credit";
                    }
                    else if (rows["Sale_Type"].ToString() == "ZBET")
                    {
                        rows["Sale_Type"] = "New";
                        rows["Type"] = "Export";
                        rows["TransactionType"] = "Export";

                    }

                    //For test
                    ////if (rows["Sale_Type"].ToString() == "ZBOR")
                    ////{
                    ////    rows["Sale_Type"] = "New";
                    ////    rows["TransactionType"] = "Other";
                    ////}
                    if (finalTable.Columns.Contains("Vehicle_No"))
                    {
                        if (string.IsNullOrWhiteSpace(rows["Vehicle_No"].ToString()))
                        {
                            rows["Vehicle_No"] = "N/A";
                        }
                    }

                    if (finalTable.Columns.Contains("INVOICE_DATE") && finalTable.Columns.Contains("INVOICE_TIME"))
                    {
                        string pattern = "yyyyMMdd HHmmss";
                        DateTime parsedDate;
                        var a = rows["INVOICE_DATE"].ToString() + " " + rows["INVOICE_TIME"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["Invoice_Date_Time"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                }
                ////finalTable.Columns.Remove("CONVERSION_UNIT");
                finalTable.Columns.Remove("CONVERSION_FACTOR");
                ////finalTable.Columns.Remove("LASNO");
                ////finalTable.Columns.Remove("PRODUCTDESCRIPTION");
                finalTable.Columns.Remove("INVOICE_DATE");
                finalTable.Columns.Remove("INVOICE_TIME");
                finalTable.Columns.Remove("PREV_INV_DATE_TIME");
                finalTable.Columns.Remove("WEIGHT_UNIT");
                finalTable.Columns.Remove("BRAND_NAME");

                //finalTable.Columns.Remove("Previous_Conversion_Factor_To_Nos");
                return finalTable;
            }
            catch (Exception e)
            {
                throw e;
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
            else if (CompanyCode.ToLower() == "PB81".ToLower())
            {
                BIN = "004207556-0508";
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

        private void bgwMaricoFactoryTransfer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetFileFactoryTransfer(FactoryTransferFile);
            }
            catch (Exception exception)
            {
                FileLogger.Log("bgwMaricoTransfer", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void bgwMaricoFactoryTransfer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isProcessRunning = false;
            progressBar1.Visible = false;
        }

        public void GetFileFactoryTransfer(List<string> File)
        {
            CommonDAL _cdal = new CommonDAL();

            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp();

            int loopCounter = 100;

            if (File.Count < loopCounter)
            {
                loopCounter = File.Count;
            }
            for (var index = 0; index < loopCounter; index++)
            {
                string file = FactoryTransferWorkPath + "/" + File[index];
                string WorkFile = File[index];
                string destinationFile = FactoryTransferPostedPath + "/" + File[index];
                string destinationHoldFile = FactoryTransferHoldPath + "/" + File[index];
                string importID = "";
                DataTable finaldata = new DataTable();
                try
                {
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FactoryTransferWorkPath + "/" + File[index]);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.Credentials = new NetworkCredential(User, Pass);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string xml = reader.ReadToEnd();
                    response.Close();

                    xml = xml.Replace("%", "");
                    xml = xml.Replace("&", "and");

                    DataTable dtTableResult = GetTableFromXML(xml);

                    dtTableResult = FormatFactoryTransferData(dtTableResult);

                    if (!dtTableResult.Columns.Contains("OtherRef"))
                    {
                        var columnName = new DataColumn("OtherRef") { DefaultValue = File[index] };
                        dtTableResult.Columns.Add(columnName);
                    }

                    TransferIssueDAL TransferIssueDal = new TransferIssueDAL();

                    #region Mouchak

                    DataRow[] rows = dtTableResult.Select("PLANT_CODE = 'PB23'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();

                            NewconnVM = Authenticate("PB23");

                            finaldata.Columns.Remove("PLANT_CODE");

                            importID = string.Join(",", finaldata
                          .DefaultView
                          .ToTable(true, "ID")
                          .Rows.Cast<DataRow>()
                          .Select(x => x["ID"].ToString()));

                            string Transaction_Type = "62Out";

                            if (finaldata.Rows.Count > 0)
                            {
                                Transaction_Type = finaldata.Rows[0]["TransactionType"].ToString();
                            }

                            try
                            {
                                if (finaldata.Rows.Count > 0)
                                {
                                    _cdal.BulkInsert("TempTransferMarico", finaldata, null, null, 0, null, NewconnVM);
                                }
                            }
                            catch (Exception e)
                            {

                            }

                            TransferIssueDal.SaveTempTransfer(finaldata, Program.BranchCode, Transaction_Type, "Process", Program.BranchId, () => { }, null, null, false, NewconnVM, ""
                                , Program.CurrentUserID);
                        }
                        catch (Exception e)
                        {
                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO", "GetFileFactoryTransfer", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB23";
                            vm.ActionName = "GetFileFactoryTransfer";
                            vm.TransactionName = "FactoryTransfer";

                            vms.Add(vm);

                            DataTable dt = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                        }

                    }

                    #endregion

                    #region Shirirchala

                    DataRow[] dtrows = dtTableResult.Select("PLANT_CODE = 'PBBO'");

                    if (dtrows.Count() > 0)
                    {
                        finaldata = new DataTable();

                        finaldata = dtrows.CopyToDataTable();

                        NewconnVM = Authenticate("PBBO");

                        finaldata.Columns.Remove("PLANT_CODE");

                        importID = string.Join(",", finaldata
                      .DefaultView
                      .ToTable(true, "ID")
                      .Rows.Cast<DataRow>()
                      .Select(x => x["ID"].ToString()));

                        string Transaction_Type = "62Out";
                        if (finaldata.Rows.Count > 0)
                        {
                            Transaction_Type = finaldata.Rows[0]["TransactionType"].ToString();
                        }

                        try
                        {
                            if (finaldata.Rows.Count > 0)
                            {
                                _cdal.BulkInsert("TempTransferMarico", finaldata, null, null, 0, null, NewconnVM);
                            }
                        }
                        catch (Exception e)
                        {

                        }

                        TransferIssueDal.SaveTempTransfer(finaldata, Program.BranchCode, Transaction_Type,
                        "Process", Program.BranchId, () => { }, null, null, false, NewconnVM, "", Program.CurrentUserID);

                    }

                    #endregion

                    #region Mirsarai

                    rows = dtTableResult.Select("PLANT_CODE = 'PB81'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();

                            NewconnVM = Authenticate("PB81");

                            finaldata.Columns.Remove("PLANT_CODE");

                            importID = string.Join(",", finaldata
                          .DefaultView
                          .ToTable(true, "ID")
                          .Rows.Cast<DataRow>()
                          .Select(x => x["ID"].ToString()));

                            string Transaction_Type = "62Out";

                            if (finaldata.Rows.Count > 0)
                            {
                                Transaction_Type = finaldata.Rows[0]["TransactionType"].ToString();
                            }

                            try
                            {
                                if (finaldata.Rows.Count > 0)
                                {
                                    _cdal.BulkInsert("TempTransferMarico", finaldata, null, null, 0, null, NewconnVM);
                                }
                            }
                            catch (Exception e)
                            {
                                ////CommonDAL _cdal = new CommonDAL();

                                ErrorLogVM vm = new ErrorLogVM();
                                List<ErrorLogVM> vms = new List<ErrorLogVM>();

                                FileLogger.Log("MARICO_PB81", "GetFileFactoryTransfer", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                                vm.ImportId = importID;
                                vm.FileName = WorkFile;
                                vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                                vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                vm.SourceName = "MARICO_PB81";
                                vm.ActionName = "GetFileFactoryTransfer";
                                vm.TransactionName = "FactoryTransfer";
                                vms.Add(vm);

                                DataTable dt = vms.ToList().ToDataTable();
                                _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);

                            }

                            TransferIssueDal.SaveTempTransfer(finaldata, Program.BranchCode, Transaction_Type, "Process", Program.BranchId, () => { }, null, null, false, NewconnVM, ""
                                , Program.CurrentUserID);
                        }
                        catch (Exception e)
                        {
                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO_PB81", "GetFileFactoryTransfer", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB81";
                            vm.ActionName = "GetFileFactoryTransfer";
                            vm.TransactionName = "FactoryTransfer";

                            vms.Add(vm);

                            DataTable dt = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                        }

                    }

                    #endregion

                    MoveFile(file, destinationFile, WorkFile, FactoryTransferLocalStorePath, importID, "FactoryTransfer");
                }
                catch (Exception e)
                {
                    try
                    {
                        MoveFile(file, destinationHoldFile, WorkFile, FactoryTransferLocalStorePath, importID, "FactoryTransfer");

                        ErrorLogVM vm = new ErrorLogVM();
                        List<ErrorLogVM> vms = new List<ErrorLogVM>();


                        FileLogger.Log("MARICO", "GetFileFactoryTransfer", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                        vm.ImportId = importID;
                        vm.FileName = WorkFile;
                        vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                        vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        vm.SourceName = "MARICO";
                        vm.ActionName = "GetFileFactoryTransfer";
                        vm.TransactionName = "FactoryTransfer";

                        vms.Add(vm);

                        DataTable dt = vms.ToList().ToDataTable();
                        _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                    }
                    catch
                    {

                    }
                }
            }

        }

        public DataTable FormatFactoryTransferData(DataTable table)
        {

            try
            {
                var finalTable = new DataTable();

                finalTable = table.Copy();

                #region Column Name Change

                finalTable.Columns["TRANSFER_FROM"].ColumnName = "BranchCode";
                finalTable.Columns["PRODUCT_CODE"].ColumnName = "ProductCode";
                finalTable.Columns["PRODUCT_NAME"].ColumnName = "ProductName";
                finalTable.Columns["QUANTITY"].ColumnName = "Quantity";
                finalTable.Columns["COST_PRICE"].ColumnName = "CostPrice";
                finalTable.Columns["TRANSFER_TO_BRANCH_CODE"].ColumnName = "TransferToBranchCode";
                finalTable.Columns["REFERENCE_NO"].ColumnName = "ReferenceNo";
                finalTable.Columns["VEHICLE_NO"].ColumnName = "VehicleNo";
                finalTable.Columns["VEHICLE_TYPE"].ColumnName = "VehicleType";
                finalTable.Columns["POST"].ColumnName = "Post";
                finalTable.Columns["VAT_RATE"].ColumnName = "VAT_Rate";
                finalTable.Columns["COMMENTS"].ColumnName = "Comments";
                finalTable.Columns["COMMENTSD"].ColumnName = "CommentsD";

                #endregion

                ProductDAL dal = new ProductDAL();

                #region Add new column

                finalTable.Columns.Remove("TRANSACTION_TYPE");

                if (!finalTable.Columns.Contains("Weight"))
                {
                    var columnName = new DataColumn("Weight") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("TransactionDateTime"))
                {
                    var columnName = new DataColumn("TransactionDateTime") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("TransactionType"))
                {
                    var columnName = new DataColumn("TransactionType") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("BranchFromRef"))
                {
                    var columnName = new DataColumn("BranchFromRef") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("BranchToRef"))
                {
                    var columnName = new DataColumn("BranchToRef") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }

                #endregion

                foreach (DataRow rows in finalTable.Rows)
                {
                    rows["ReferenceNo"] = rows["ID"].ToString();

                    if (string.IsNullOrWhiteSpace(rows["CONVERSION_FACTOR"].ToString()) || rows["CONVERSION_FACTOR"].ToString().Trim() == "1")
                    {
                        rows["Weight"] = "0";
                    }
                    else
                    {
                        rows["Weight"] = rows["Quantity"].ToString();
                    }

                    ////if (rows["ProductCode"].ToString() == "301111" || rows["ProductCode"].ToString() == "300804")
                    ////{
                    ////    rows["UOM"] = "L";
                    ////    rows["CONVERSION_FACTOR"] = "1.0989";
                    ////}

                    if (string.IsNullOrWhiteSpace(rows["CONVERSION_FACTOR"].ToString()))
                    {
                        ////rows["UOM"] = "Pcs";
                    }
                    else
                    {
                        ////rows["UOM"] = "Pcs";
                        rows["Quantity"] = Convert.ToDecimal(rows["Quantity"]) * Convert.ToDecimal(rows["CONVERSION_FACTOR"]);
                    }

                    rows["Post"] = "N";

                    if (finalTable.Columns.Contains("TRANSACTION_DATE") && finalTable.Columns.Contains("TRANSACTION_TIME"))
                    {
                        string pattern = "yyyyMMdd HHmmss";
                        DateTime parsedDate;
                        var a = rows["TRANSACTION_DATE"].ToString() + " " + rows["TRANSACTION_TIME"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["TransactionDateTime"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else if (finalTable.Columns.Contains("TRANSACTION_DATE"))
                    {
                        string pattern = "yyyyMMdd";
                        DateTime parsedDate;
                        var a = rows["TRANSACTION_DATE"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["TransactionDateTime"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    if (finalTable.Columns.Contains("BranchCode"))
                    {
                        rows["BranchFromRef"] = rows["BranchCode"].ToString();
                    }
                    if (finalTable.Columns.Contains("TransferToBranchCode"))
                    {
                        rows["BranchToRef"] = rows["TransferToBranchCode"].ToString();
                    }

                    string Transaction_Type = rows["MATERIAL_TYPE"].ToString();
                    string ProductType = "";

                    if (Transaction_Type.ToLower() == "FERT".ToLower())
                    {
                        Transaction_Type = "62Out";
                        ProductType = "Finish";
                    }
                    else if (Transaction_Type.ToLower() == "HAWA".ToLower())
                    {
                        Transaction_Type = "62Out";
                        ProductType = "Trading";
                    }
                    else
                    {
                        Transaction_Type = "61Out";
                        ProductType = "Raw";
                    }

                    rows["TransactionType"] = Transaction_Type;
                    rows["VAT_Rate"] = "0";

                    //////var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { rows["ProductCode"].ToString() });

                    //////ProductVM vm = vms.FirstOrDefault();

                    //////if (vm == null)
                    //////{
                    //////    try
                    //////    {
                    //////        CommonDAL com = new CommonDAL();
                    //////        CommonImportDAL cImport = new CommonImportDAL();
                    //////        cImport.FindItemId(rows["ProductName"].ToString(), rows["ProductCode"].ToString(), null, null, true, "Pcs", 1, 15, 1, null, ProductType);
                    //////        vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { rows["ProductCode"].ToString() });
                    //////        vm = vms.FirstOrDefault();

                    //////    }
                    //////    catch
                    //////    {

                    //////    }
                    //////}

                    //////if (Transaction_Type == "62Out")
                    //////{
                    //////    rows["CostPrice"] = vm.NBRPrice;
                    //////}
                    //////else
                    //////{
                    //////    string NBRPrice = "0";
                    //////    DataTable priceData = dal.AvgPriceNew(vm.ItemNo, rows["TransactionDateTime"].ToString(), null
                    //////        , null, true, true, true, true, connVM, Program.CurrentUserID);
                    //////    decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    //////    decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                    //////    if (quan > 0)
                    //////    {
                    //////        NBRPrice = (amount / quan).ToString();
                    //////    }
                    //////    else
                    //////    {
                    //////        NBRPrice = "0";
                    //////    }
                    //////    rows["CostPrice"] = Convert.ToDecimal(NBRPrice);
                    //////}

                    if (string.IsNullOrEmpty(rows["VehicleNo"].ToString()) || rows["VehicleNo"].ToString() == "-")
                    {
                        rows["VehicleNo"] = "NA";
                    }
                    if (string.IsNullOrEmpty(rows["VehicleType"].ToString()) || rows["VehicleType"].ToString() == "-")
                    {
                        rows["VehicleType"] = "NA";
                    }

                }

                finalTable.Columns.Remove("TRANSACTION_DATE");
                finalTable.Columns.Remove("TRANSACTION_TIME");
                finalTable.Columns.Remove("MATERIAL_TYPE");
                finalTable.Columns.Remove("CONVERSION_FACTOR");

                return finalTable;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private void bgwMaricoContractManufacturing_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                GetFileContractManufacturingFactory(FactoryContractManufacturingFile);
            }
            catch (Exception exception)
            {
                FileLogger.Log("ContractManufacturingImportMARICO", "bgwMaricoContractManufacturing_DoWork", exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void bgwMaricoContractManufacturing_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isProcessRunning = false;
            progressBar1.Visible = false;
        }

        public void GetFileContractManufacturingFactory(List<string> File)
        {
            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp();

            int loopCounter = 100;

            if (File.Count < loopCounter)
            {
                loopCounter = File.Count;
            }
            for (var index = 0; index < loopCounter; index++)
            {
                string file = ContractManufacturingWorkPath + "/" + File[index];
                string WorkFile = File[index];
                string destinationFile = ContractManufacturingPostedPath + "/" + File[index];
                string destinationHoldFile = ContractManufacturingHoldPath + "/" + File[index];

                string importID = "";
                DataTable finaldata = new DataTable();

                try
                {
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ContractManufacturingWorkPath + "/" + File[index]);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.Credentials = new NetworkCredential(User, Pass);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string xml = reader.ReadToEnd();
                    response.Close();
                    xml = xml.Replace("&", "And");
                    xml = xml.Replace("%", "");

                    DataTable dtsale = GetTableFromXML(xml);

                    //dtsale = FormatContractManufacturingFactoryData(dtsale);

                    //TableValidation(dtsale, File[index]);

                    SaleDAL saleDal = new SaleDAL();

                    #region Mouchak

                    DataRow[] rows = dtsale.Select("PLANT_CODE = 'PB23'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();

                            NewconnVM = Authenticate("PB23");

                            finaldata = FormatContractManufacturingFactoryData(finaldata);
                            TableValidation(finaldata);

                            importID = string.Join(",", finaldata
                               .DefaultView
                               .ToTable(true, "ID")
                               .Rows.Cast<DataRow>()
                               .Select(x => x["ID"].ToString()));

                            finaldata.Columns.Remove("PLANT_CODE");

                            saleDal.SaveAndProcess(finaldata, () => { }, Program.BranchId, "", NewconnVM);
                        }
                        catch (Exception e)
                        {
                            CommonDAL _cdal = new CommonDAL();

                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO", "ContractManufacturingFactory", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB23";
                            vm.ActionName = "GetFileContractManufacturingFactory";
                            vm.TransactionName = "ContractManufacturing";
                            vms.Add(vm);

                            DataTable dt = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);

                        }

                    }

                    #endregion

                    #region Shirirchala

                    DataRow[] dtrows = dtsale.Select("PLANT_CODE = 'PBBO'");

                    if (dtrows.Count() > 0)
                    {
                        try
                        {
                            finaldata = new DataTable();

                            finaldata = dtrows.CopyToDataTable();

                            NewconnVM = Authenticate("PBBO");

                            finaldata = FormatContractManufacturingFactoryData(finaldata);
                            TableValidation(finaldata);

                            importID = string.Join(",", finaldata
                           .DefaultView
                           .ToTable(true, "ID")
                           .Rows.Cast<DataRow>()
                           .Select(x => x["ID"].ToString()));

                            finaldata.Columns.Remove("PLANT_CODE");

                            saleDal.SaveAndProcess(finaldata, () => { }, Program.BranchId, "", NewconnVM);
                        }
                        catch (Exception e)
                        {
                            CommonDAL _cdal = new CommonDAL();

                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO_PBBO", "ContractManufacturingFactory", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PBBO";
                            vm.ActionName = "GetFileContractManufacturingFactory";
                            vm.TransactionName = "ContractManufacturing";
                            vms.Add(vm);

                            DataTable dt = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);

                        }
                    }

                    #endregion

                    #region Mirsarai

                    rows = dtsale.Select("PLANT_CODE = 'PB81'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();

                            NewconnVM = Authenticate("PB81");

                            finaldata = FormatContractManufacturingFactoryData(finaldata);
                            TableValidation(finaldata);

                            importID = string.Join(",", finaldata
                               .DefaultView
                               .ToTable(true, "ID")
                               .Rows.Cast<DataRow>()
                               .Select(x => x["ID"].ToString()));

                            finaldata.Columns.Remove("PLANT_CODE");

                            saleDal.SaveAndProcess(finaldata, () => { }, Program.BranchId, "", NewconnVM);
                        }
                        catch (Exception e)
                        {
                            CommonDAL _cdal = new CommonDAL();

                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO_PB81", "GetFileSalesFactory", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB81";
                            vm.ActionName = "GetFileContractManufacturingFactory";
                            vm.TransactionName = "ContractManufacturing";
                            vms.Add(vm);

                            DataTable dt = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);

                        }

                    }

                    #endregion

                    MoveFile(file, destinationFile, WorkFile, ContractManufacturingStorePath, importID, "ContractManufacturing");

                }
                catch (Exception e)
                {
                    try
                    {

                        CommonDAL _cdal = new CommonDAL();
                        MoveFile(file, destinationHoldFile, WorkFile, ContractManufacturingStorePath, importID, "ContractManufacturing");

                        ErrorLogVM vm = new ErrorLogVM();
                        List<ErrorLogVM> vms = new List<ErrorLogVM>();

                        FileLogger.Log("MARICO", "GetFileSalesFactory", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                        vm.ImportId = importID;
                        vm.FileName = WorkFile;
                        vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                        vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        vm.SourceName = "MARICO";
                        vm.ActionName = "GetFileContractManufacturingFactory";
                        vm.TransactionName = "ContractManufacturing";
                        vms.Add(vm);

                        DataTable dt = vms.ToList().ToDataTable();
                        _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                    }
                    catch
                    {

                    }
                }
            }

        }

        public DataTable FormatContractManufacturingFactoryData(DataTable table)
        {
            ProductDAL dal = new ProductDAL();

            try
            {
                var finalTable = new DataTable();

                finalTable = table.Copy();

                #region Column Name Change

                ////finalTable.Columns["PREV_INV_DATE_TIME"].ColumnName = "PreviousInvoiceDateTime";
                finalTable.Columns["TRANSFER_FROM"].ColumnName = "Branch_Code";
                finalTable.Columns["PRODUCT_CODE"].ColumnName = "Item_Code";
                finalTable.Columns["PRODUCT_NAME"].ColumnName = "Item_Name";
                finalTable.Columns["QUANTITY"].ColumnName = "Quantity";
                finalTable.Columns["TRANSFER_TO_BRANCH_CODE"].ColumnName = "Customer_Code";
                finalTable.Columns["REFERENCE_NO"].ColumnName = "Reference_No";
                finalTable.Columns["VEHICLE_NO"].ColumnName = "Vehicle_No";
                finalTable.Columns["VEHICLE_TYPE"].ColumnName = "VehicleType";
                finalTable.Columns["TRANSACTION_TYPE"].ColumnName = "TransactionType";
                finalTable.Columns["POST"].ColumnName = "Post";
                finalTable.Columns["VAT_RATE"].ColumnName = "VAT_Rate";
                finalTable.Columns["COMMENTS"].ColumnName = "Comments";
                finalTable.Columns["COMMENTSD"].ColumnName = "CommentsD";

                #endregion

                #region New Column add
                if (!finalTable.Columns.Contains("Customer_Name"))
                {
                    var columnName = new DataColumn("Customer_Name") { DefaultValue = "-" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("NBR_Price"))
                {
                    var columnName = new DataColumn("NBR_Price") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Invoice_Date_Time"))
                {
                    var columnName = new DataColumn("Invoice_Date_Time") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }

                if (!finalTable.Columns.Contains("PreviousInvoiceDateTime"))
                {
                    var columnName = new DataColumn("PreviousInvoiceDateTime") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("CustomerBIN"))
                {
                    var columnName = new DataColumn("CustomerBIN") { DefaultValue = "XNAX" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Weight"))
                {
                    var columnName = new DataColumn("Weight") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Sale_Type"))
                {
                    var columnName = new DataColumn("Sale_Type") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }

                if (!finalTable.Columns.Contains("Is_Print"))
                {
                    var columnName = new DataColumn("Is_Print") { DefaultValue = "N" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Tender_Id"))
                {
                    var columnName = new DataColumn("Tender_Id") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Currency_Code"))
                {
                    var columnName = new DataColumn("Currency_Code") { DefaultValue = "BDT" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("SD_Rate"))
                {
                    var columnName = new DataColumn("SD_Rate") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("SDAmount"))
                {
                    var columnName = new DataColumn("SDAmount") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Non_Stock"))
                {
                    var columnName = new DataColumn("Non_Stock") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Trading_MarkUp"))
                {
                    var columnName = new DataColumn("Trading_MarkUp") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Discount_Amount"))
                {
                    var columnName = new DataColumn("Discount_Amount") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Promotional_Quantity"))
                {
                    var columnName = new DataColumn("Promotional_Quantity") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("VAT_Name"))
                {
                    var columnName = new DataColumn("VAT_Name") { DefaultValue = "VAT 4.3" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Previous_Invoice_No"))
                {
                    var columnName = new DataColumn("Previous_Invoice_No") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("PreviousNBRPrice"))
                {
                    var columnName = new DataColumn("PreviousNBRPrice") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("PreviousQuantity"))
                {
                    var columnName = new DataColumn("PreviousQuantity") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("PreviousSubTotal"))
                {
                    var columnName = new DataColumn("PreviousSubTotal") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("PreviousSD"))
                {
                    var columnName = new DataColumn("PreviousSD") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("PreviousSDAmount"))
                {
                    var columnName = new DataColumn("PreviousSDAmount") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("PreviousVATRate"))
                {
                    var columnName = new DataColumn("PreviousVATRate") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("PreviousVATAmount"))
                {
                    var columnName = new DataColumn("PreviousVATAmount") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("SubTotal"))
                {
                    var columnName = new DataColumn("SubTotal") { DefaultValue = "0" };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Type"))
                {
                    var columnName = new DataColumn("Type") { DefaultValue = "" };
                    finalTable.Columns.Add(columnName);
                }

                #endregion

                foreach (DataRow rows in finalTable.Rows)
                {
                    ////if (rows["Item_Code"].ToString() == "301111" || rows["Item_Code"].ToString() == "300804")
                    ////{
                    ////    rows["UOM"] = "L";
                    ////    rows["CONVERSION_FACTOR"] = "1.0989";

                    ////}

                    if (string.IsNullOrWhiteSpace(rows["PreviousInvoiceDateTime"].ToString()) || rows["PreviousInvoiceDateTime"].ToString() == "0")
                    {
                        rows["PreviousInvoiceDateTime"] = "1900-01-01 00:00:00";

                    }

                    rows["Sale_Type"] = "New";
                    rows["TransactionType"] = "TollIssue";

                    if (finalTable.Columns.Contains("Vehicle_No"))
                    {
                        if (string.IsNullOrWhiteSpace(rows["Vehicle_No"].ToString()))
                        {
                            rows["Vehicle_No"] = "N/A";
                        }
                    }

                    if (finalTable.Columns.Contains("TRANSACTION_DATE") && finalTable.Columns.Contains("TRANSACTION_TIME"))
                    {
                        if (!string.IsNullOrWhiteSpace(rows["TRANSACTION_DATE"].ToString()) && !string.IsNullOrWhiteSpace(rows["TRANSACTION_TIME"].ToString()))
                        {
                            string pattern = "yyyyMMdd HHmmss";
                            DateTime parsedDate;
                            var a = rows["TRANSACTION_DATE"].ToString() + " " + rows["TRANSACTION_TIME"].ToString();
                            DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                            rows["Invoice_Date_Time"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                    }
                    else if (finalTable.Columns.Contains("TRANSACTION_DATE"))
                    {
                        if (!string.IsNullOrWhiteSpace(rows["TRANSACTION_DATE"].ToString()))
                        {
                            string pattern = "yyyyMMdd";
                            DateTime parsedDate;
                            var a = rows["TRANSACTION_DATE"].ToString();
                            DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                            rows["Invoice_Date_Time"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }

                    var vms = dal.SelectAll("0", new string[] { "Pr.ProductCode" }, new[] { rows["Item_Code"].ToString() });
                    ProductVM vm = vms.FirstOrDefault();

                    if (vm != null)
                    {
                        string NBRPrice = "0";
                        DataTable priceData = dal.AvgPriceNew(vm.ItemNo, rows["Invoice_Date_Time"].ToString(), null, null, true, true, true, true, connVM, Program.CurrentUserID);

                        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                        if (quan > 0)
                        {
                            NBRPrice = (amount / quan).ToString();
                        }
                        else
                        {
                            NBRPrice = "0";
                        }
                        rows["NBR_Price"] = (Convert.ToDecimal(rows["Quantity"])) * Convert.ToDecimal(NBRPrice);

                    }
                    else
                    {
                        rows["NBR_Price"] = "0";

                    }

                    if (!string.IsNullOrWhiteSpace(rows["VAT_Rate"].ToString()))
                    {
                        if (Convert.ToDecimal(rows["VAT_Rate"].ToString()) == 0)
                        {
                            rows["Type"] = "NonVat";
                        }
                        else if (Convert.ToDecimal(rows["VAT_Rate"].ToString()) == 15)
                        {
                            rows["Type"] = "VAT";
                        }
                        else
                        {
                            rows["Type"] = "OtherRate";
                        }
                    }
                    else
                    {
                        rows["Type"] = "NonVat";
                    }

                    if (!string.IsNullOrWhiteSpace(rows["CONVERSION_FACTOR"].ToString()) && rows["UOM"].ToString().ToLower() != "nos")
                    {
                        rows["Quantity"] = Convert.ToDecimal(rows["Quantity"]) * Convert.ToDecimal(rows["CONVERSION_FACTOR"]);
                        ////rows["NBR_Price"] = Convert.ToDecimal(rows["NBR_Price"]) / Convert.ToDecimal(rows["CONVERSION_FACTOR"]);
                    }

                    rows["Reference_No"] = rows["ID"];
                }

                finalTable.Columns.Remove("CONVERSION_FACTOR");
                finalTable.Columns.Remove("TRANSACTION_DATE");
                finalTable.Columns.Remove("TRANSACTION_TIME");

                return finalTable;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void bgwMARICOFactoryPurchase_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {

                GetFileFactoryPurchase(FactoryPurchaseFile);
            }
            catch (Exception exception)
            {
                FileLogger.Log("bgwMARICOFactoryPurchase", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);
            }

        }

        private void bgwMARICOFactoryPurchase_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isProcessRunning = false;
            progressBar1.Visible = false;
        }

        public void GetFileFactoryPurchase(List<string> File)
        {
            CommonDAL _cdal = new CommonDAL();

            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp();

            int loopCounter = 100;

            if (File.Count < loopCounter)
            {
                loopCounter = File.Count;
            }
            for (var index = 0; index < loopCounter; index++)
            {
                string file = FactoryPurchaseWorkPath + "/" + File[index];
                string WorkFile = File[index];
                string destinationFile = FactoryPurchasePostedPath + "/" + File[index];
                string destinationHoldFile = FactoryPurchaseHoldPath + "/" + File[index];
                string importID = "";
                DataTable finaldata = new DataTable();

                try
                {
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FactoryPurchaseWorkPath + "/" + File[index]);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.Credentials = new NetworkCredential(User, Pass);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string xml = reader.ReadToEnd();
                    xml = xml.Replace("&", "And");
                    response.Close();

                    DataTable dtpurchase = GetTableFromXML(xml);

                    dtpurchase = FormatFactoryPurchaseData(dtpurchase);

                    PurchaseDAL purchaseDal = new PurchaseDAL();

                    var OtherRef = new DataColumn("OtherRef") { DefaultValue = WorkFile };
                    dtpurchase.Columns.Add(OtherRef);

                    #region Mouchak

                    DataRow[] rows = dtpurchase.Select("BranchCode = 'PB23'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();

                            NewconnVM = Authenticate("PB23");

                            importID = string.Join(",", finaldata
                            .DefaultView
                            .ToTable(true, "ID")
                            .Rows.Cast<DataRow>()
                            .Select(x => x["ID"].ToString()));
                            try
                            {
                                if (finaldata.Rows.Count > 0)
                                {
                                    _cdal.BulkInsert("TempPurchaseMarico", finaldata, null, null, 0, null, NewconnVM);
                                }
                            }
                            catch (Exception e)
                            {

                            }

                            purchaseDal.SaveTempPurchase(finaldata, Program.BranchCode, "Other", "Process", Program.BranchId, () => { }, null, null, NewconnVM, "", false);

                        }
                        catch (Exception e)
                        {
                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO", "GetFileFactoryPurchase", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB23";
                            vm.ActionName = "GetFileFactoryPurchase";
                            vm.TransactionName = "FactoryPurchase";

                            vms.Add(vm);

                            DataTable dt = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                        }

                    }

                    #endregion

                    #region Mirsarai

                    rows = dtpurchase.Select("BranchCode = 'PB81'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();

                            NewconnVM = Authenticate("PB81");

                            importID = string.Join(",", finaldata
                            .DefaultView
                            .ToTable(true, "ID")
                            .Rows.Cast<DataRow>()
                            .Select(x => x["ID"].ToString()));
                            try
                            {
                                if (finaldata.Rows.Count > 0)
                                {
                                    _cdal.BulkInsert("TempPurchaseMarico", finaldata, null, null, 0, null, NewconnVM);
                                }
                            }
                            catch (Exception e)
                            {

                            }

                            purchaseDal.SaveTempPurchase(finaldata, Program.BranchCode, "Other", "Process", Program.BranchId, () => { }, null, null, NewconnVM, "", false);

                        }
                        catch (Exception e)
                        {
                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO_PB81", "GetFileFactoryPurchase", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB81";
                            vm.ActionName = "GetFileFactoryPurchase";
                            vm.TransactionName = "FactoryPurchase";

                            vms.Add(vm);

                            DataTable dt = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                        }

                    }

                    #endregion

                    #region Shirirchala

                    DataRow[] dtrows = dtpurchase.Select("BranchCode = 'PBBO'");

                    if (dtrows.Count() > 0)
                    {
                        finaldata = new DataTable();

                        finaldata = dtrows.CopyToDataTable();

                        NewconnVM = Authenticate("PBBO");

                        importID = string.Join(",", finaldata
                         .DefaultView
                         .ToTable(true, "ID")
                         .Rows.Cast<DataRow>()
                         .Select(x => x["ID"].ToString()));
                        try
                        {
                            if (finaldata.Rows.Count > 0)
                            {
                                _cdal.BulkInsert("TempPurchaseMarico", finaldata, null, null, 0, null, connVM);
                            }
                        }
                        catch (Exception e)
                        {

                        }

                        purchaseDal.SaveTempPurchase(finaldata, Program.BranchCode, "Other", "Process", Program.BranchId, () => { }, null, null, NewconnVM, "", false);

                    }

                    #endregion

                    MoveFile(file, destinationFile, WorkFile, FactoryPurchaseLocalStorePath, importID, "FactoryPurchase");

                }
                catch (Exception e)
                {
                    try
                    {
                        MoveFile(file, destinationHoldFile, WorkFile, FactoryPurchaseLocalStorePath, importID, "FactoryPurchase");

                        ErrorLogVM vm = new ErrorLogVM();
                        List<ErrorLogVM> vms = new List<ErrorLogVM>();

                        FileLogger.Log("MARICO", "GetFileFactoryPurchase", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                        vm.ImportId = importID;
                        vm.FileName = WorkFile;
                        vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                        vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        vm.SourceName = "MARICO";
                        vm.ActionName = "GetFileFactoryPurchase";
                        vm.TransactionName = "FactoryPurchase";

                        vms.Add(vm);

                        DataTable dt = vms.ToList().ToDataTable();
                        _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                    }
                    catch
                    {
                    }
                }
            }

        }

        public static DataTable FormatFactoryPurchaseData(DataTable table)
        {

            try
            {
                var finalTable = new DataTable();
                string DateNow = DateTime.Now.ToString("yyyy-MM-dd");
                string timeNow = DateTime.Now.ToString("HH:mm:ss");

                finalTable = table.Copy();

                #region Column Name Change

                finalTable.Columns["PLANT_CODE"].ColumnName = "BranchCode";
                finalTable.Columns["VENDOR_CODE"].ColumnName = "Vendor_Code";
                finalTable.Columns["VENDOR_NAME"].ColumnName = "Vendor_Name";
                finalTable.Columns["REFERENCE_NO"].ColumnName = "Referance_No";
                finalTable.Columns["LC_NO"].ColumnName = "LC_No";
                finalTable.Columns["BE_NUMBER"].ColumnName = "BE_Number";
                finalTable.Columns["POST"].ColumnName = "Post";
                finalTable.Columns["WITH_VDS"].ColumnName = "With_VDS";
                finalTable.Columns["COMMENTS"].ColumnName = "Comments";
                finalTable.Columns["TRANSACTION_TYPE"].ColumnName = "Transection_Type";
                finalTable.Columns["CUSTOM_HOUSE"].ColumnName = "Custom_House";
                finalTable.Columns["ITEM_NAME"].ColumnName = "Item_Name";
                finalTable.Columns["ITEM_CODE"].ColumnName = "Item_Code";
                finalTable.Columns["PRODUCT_GROUP"].ColumnName = "Product_Group";
                finalTable.Columns["QUANTITY"].ColumnName = "Quantity";
                finalTable.Columns["TOTAL_PRICE"].ColumnName = "Total_Price";
                finalTable.Columns["TYPE"].ColumnName = "Type";
                finalTable.Columns["FIXED_VAT_REBATE"].ColumnName = "FixedVATRebate";
                finalTable.Columns["REBATE_RATE"].ColumnName = "Rebate_Rate";
                finalTable.Columns["SD_AMOUNT"].ColumnName = "SD_Amount";
                finalTable.Columns["VAT_AMOUNT"].ColumnName = "VAT_Amount";
                finalTable.Columns["VDS_AMOUNT"].ColumnName = "VDSAmount";
                finalTable.Columns["CNF_AMOUNT"].ColumnName = "CnF_Amount";
                finalTable.Columns["INSURANCE_AMOUNT"].ColumnName = "Insurance_Amount";
                finalTable.Columns["ASSESSABLE_VALUE"].ColumnName = "Assessable_Value";
                finalTable.Columns["CD_AMOUNT"].ColumnName = "CD_Amount";
                finalTable.Columns["RD_AMOUNT"].ColumnName = "RD_Amount";
                finalTable.Columns["AIT_AMOUNT"].ColumnName = "AITAmount";
                finalTable.Columns["AT_AMOUNT"].ColumnName = "AT_Amount";
                finalTable.Columns["OTHERS_AMOUNT"].ColumnName = "Others_Amount";
                finalTable.Columns["REMARKS"].ColumnName = "Remarks";
                finalTable.Columns["ISREBATE"].ColumnName = "IsRebate";
                finalTable.Columns["INVOICE_DATE"].ColumnName = "Invoice_Date";
                finalTable.Columns["RECEIVE_DATE"].ColumnName = "Receive_Date";
                finalTable.Columns["REBATE_DATE"].ColumnName = "Rebate_Date";

                #endregion

                #region New Column add


                if (!finalTable.Columns.Contains("Invoice_Time"))
                {
                    var columnName = new DataColumn("Invoice_Time") { DefaultValue = timeNow };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Rebate_Time"))
                {
                    var columnName = new DataColumn("Rebate_Time") { DefaultValue = timeNow };
                    finalTable.Columns.Add(columnName);
                }
                if (!finalTable.Columns.Contains("Receive_Time"))
                {
                    var columnName = new DataColumn("Receive_Time") { DefaultValue = timeNow };
                    finalTable.Columns.Add(columnName);
                }

                #endregion

                ProductDAL dal = new ProductDAL();

                foreach (DataRow rows in finalTable.Rows)
                {
                    ////if (rows["Item_Code"].ToString() == "301111" || rows["Item_Code"].ToString() == "300804")
                    ////{
                    ////    rows["UOM"] = "L";
                    ////    rows["CONVERSION_FACTOR"] = "1.0989";

                    ////}

                    if (string.IsNullOrWhiteSpace(rows["CONVERSION_FACTOR"].ToString()))
                    {
                        ////rows["UOM"] = "Pcs";
                    }
                    else
                    {
                        ////rows["UOM"] = "Pcs";
                        rows["Quantity"] = Convert.ToDecimal(rows["Quantity"]) * Convert.ToDecimal(rows["CONVERSION_FACTOR"]);
                    }

                    rows["Post"] = "N";

                    if (string.IsNullOrWhiteSpace(rows["Total_Price"].ToString()))
                    {
                        rows["Total_Price"] = 1;
                    }
                    else if (Convert.ToDecimal(rows["Total_Price"]) == 0)
                    {
                        rows["Total_Price"] = 1;
                    }

                    if (string.IsNullOrWhiteSpace(rows["Item_Code"].ToString()) && string.IsNullOrWhiteSpace(rows["Item_Name"].ToString()))
                    {
                        rows["Item_Code"] = "NA";
                        rows["Item_Name"] = "NA";
                        rows["Transection_Type"] = "InputService";

                        if (string.IsNullOrWhiteSpace(rows["Quantity"].ToString()))
                        {
                            rows["Quantity"] = 1;
                        }
                        else if (Convert.ToDecimal(rows["Quantity"]) == 0)
                        {
                            rows["Quantity"] = 1;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(rows["Vendor_Code"].ToString()) && string.IsNullOrWhiteSpace(rows["Vendor_Name"].ToString()))
                    {
                        rows["Vendor_Code"] = "N/A";
                        rows["Vendor_Name"] = "N/A";
                    }

                    string tType = rows["Transection_Type"].ToString();

                    if (tType.ToLower() == "Local".ToLower() || string.IsNullOrWhiteSpace(tType))
                    {
                        rows["Transection_Type"] = "Other";
                    }
                    else if (tType.ToLower() == "Foreign".ToLower())
                    {
                        rows["Transection_Type"] = "Import";
                    }

                    rows["Product_Group"] = "Finish";


                    if (finalTable.Columns.Contains("Invoice_Date"))
                    {
                        if (string.IsNullOrWhiteSpace(rows["Invoice_Date"].ToString()) || rows["Invoice_Date"].ToString() == "00000000")
                        {
                            rows["Invoice_Date"] = DateNow;
                        }
                        else
                        {
                            string pattern = "yyyyMMdd";
                            DateTime parsedDate;
                            var a = rows["Invoice_Date"].ToString();
                            DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                            rows["Invoice_Date"] = parsedDate.ToString("yyyy-MM-dd");
                        }

                    }
                    if (finalTable.Columns.Contains("Receive_Date"))
                    {
                        if (string.IsNullOrWhiteSpace(rows["Receive_Date"].ToString()) || rows["Receive_Date"].ToString() == "00000000")
                        {
                            rows["Receive_Date"] = DateNow;
                        }
                        else
                        {
                            string pattern = "yyyyMMdd";
                            DateTime parsedDate;
                            var a = rows["Receive_Date"].ToString();
                            DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                            rows["Receive_Date"] = parsedDate.ToString("yyyy-MM-dd");
                        }
                    }
                    if (finalTable.Columns.Contains("Rebate_Date"))
                    {
                        if (string.IsNullOrWhiteSpace(rows["Rebate_Date"].ToString()) || rows["Rebate_Date"].ToString() == "00000000")
                        {
                            rows["Rebate_Date"] = DateNow;
                        }
                        else
                        {
                            string pattern = "yyyyMMdd";
                            DateTime parsedDate;
                            var a = rows["Rebate_Date"].ToString();
                            DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                            rows["Rebate_Date"] = parsedDate.ToString("yyyy-MM-dd");
                        }
                    }
                }

                finalTable.Columns.Remove("CONVERSION_FACTOR");
                finalTable.Columns.Remove("INVOICE_VALUE");
                finalTable.Columns.Remove("EXCHANGE_RATE");
                finalTable.Columns.Remove("CURRENCY");

                return finalTable;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void bgwMARICOFactoryIssue_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                GetFileFactoryIssue(FactoryIssueFile);
            }
            catch (Exception exception)
            {
                FileLogger.Log("bgwMARICOFactoryIssue", "btnBulk_Click", exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void bgwMARICOFactoryIssue_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isProcessRunning = false;
            progressBar1.Visible = false;
        }

        public void GetFileFactoryIssue(List<string> File)
        {
            CommonDAL _cdal = new CommonDAL();

            SysDBInfoVMTemp NewconnVM = new SysDBInfoVMTemp();

            int loopCounter = 100;

            if (File.Count < loopCounter)
            {
                loopCounter = File.Count;
            }
            for (var index = 0; index < loopCounter; index++)
            {
                string file = FactoryIssueWorkPath + "/" + File[index];
                string WorkFile = File[index];
                string destinationFile = FactoryIssuePostedPath + "/" + File[index];
                string destinationHoldFile = FactoryIssueHoldPath + "/" + File[index];
                string importID = "";
                DataTable finaldata = new DataTable();
                IssueDAL issueDal = new IssueDAL();

                try
                {
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(FactoryIssueWorkPath + "/" + File[index]);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.Credentials = new NetworkCredential(User, Pass);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string xml = reader.ReadToEnd();
                    xml = xml.Replace("&", "And");
                    response.Close();

                    DataTable dt = GetTableFromXML(xml);

                    dt = FormatFactoryIssueData(dt);

                    #region Mouchak

                    DataRow[] rows = dt.Select("BranchCode = 'PB23'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();

                            NewconnVM = Authenticate("PB23");

                            importID = string.Join(",", finaldata
                            .DefaultView
                            .ToTable(true, "ID")
                            .Rows.Cast<DataRow>()
                            .Select(x => x["ID"].ToString()));

                            IntegrationParam param = new IntegrationParam();
                            param.TransactionType = "Other";
                            param.CurrentUser = "Process";
                            param.Data = finaldata;

                            issueDal.SaveIssue_Split(param, NewconnVM);

                        }
                        catch (Exception e)
                        {
                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO", "GetFileFactoryIssue", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB23";
                            vm.ActionName = "GetFileFactoryIssue";
                            vm.TransactionName = "FactoryIssue";

                            vms.Add(vm);

                            DataTable dtError = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dtError, null, null, 0, null, NewconnVM);
                        }

                    }

                    #endregion

                    #region Mirsarai

                    rows = dt.Select("BranchCode = 'PB81'");

                    if (rows.Count() > 0)
                    {
                        try
                        {
                            finaldata = rows.CopyToDataTable();

                            NewconnVM = Authenticate("PB81");

                            importID = string.Join(",", finaldata
                            .DefaultView
                            .ToTable(true, "ID")
                            .Rows.Cast<DataRow>()
                            .Select(x => x["ID"].ToString()));

                            IntegrationParam param = new IntegrationParam();
                            param.TransactionType = "Other";
                            param.CurrentUser = "Process";
                            param.Data = finaldata;

                            issueDal.SaveIssue_Split(param, NewconnVM);

                        }
                        catch (Exception e)
                        {
                            ErrorLogVM vm = new ErrorLogVM();
                            List<ErrorLogVM> vms = new List<ErrorLogVM>();

                            FileLogger.Log("MARICO_PB81", "GetFileFactoryIssue", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                            vm.ImportId = importID;
                            vm.FileName = WorkFile;
                            vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                            vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            vm.SourceName = "MARICO_PB81";
                            vm.ActionName = "GetFileFactoryIssue";
                            vm.TransactionName = "FactoryIssue";

                            vms.Add(vm);

                            DataTable dtError = vms.ToList().ToDataTable();
                            _cdal.BulkInsert("ErrorLogs", dtError, null, null, 0, null, NewconnVM);
                        }

                    }

                    #endregion

                    #region Shirirchala

                    DataRow[] dtrows = dt.Select("BranchCode = 'PBBO'");

                    if (dtrows.Count() > 0)
                    {
                        finaldata = new DataTable();

                        finaldata = dtrows.CopyToDataTable();

                        NewconnVM = Authenticate("PBBO");

                        importID = string.Join(",", finaldata
                         .DefaultView
                         .ToTable(true, "ID")
                         .Rows.Cast<DataRow>()
                         .Select(x => x["ID"].ToString()));

                        IntegrationParam param = new IntegrationParam();
                        param.TransactionType = "Other";
                        param.CurrentUser = "Process";
                        param.Data = finaldata;

                        issueDal.SaveIssue_Split(param, NewconnVM);

                    }

                    #endregion

                    MoveFile(file, destinationFile, WorkFile, FactoryIssueLocalStorePath, importID, "FactoryIssue");

                }
                catch (Exception e)
                {
                    try
                    {
                        MoveFile(file, destinationHoldFile, WorkFile, FactoryIssueLocalStorePath, importID, "FactoryIssue");

                        ErrorLogVM vm = new ErrorLogVM();
                        List<ErrorLogVM> vms = new List<ErrorLogVM>();

                        FileLogger.Log("MARICO", "GetFileFactoryIssue", File[index] + "\n" + e.Message + "\n" + e.StackTrace);

                        vm.ImportId = importID;
                        vm.FileName = WorkFile;
                        vm.ErrorMassage = e.Message.ToString() + "\n" + e.StackTrace.ToString();
                        vm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        vm.SourceName = "MARICO";
                        vm.ActionName = "GetFileFactoryIssue";
                        vm.TransactionName = "FactoryIssue";

                        vms.Add(vm);

                        DataTable dt = vms.ToList().ToDataTable();
                        _cdal.BulkInsert("ErrorLogs", dt, null, null, 0, null, NewconnVM);
                    }
                    catch
                    {
                    }
                }
            }

        }

        public static DataTable FormatFactoryIssueData(DataTable table)
        {

            try
            {
                var finalTable = new DataTable();
                string DateNow = DateTime.Now.ToString("yyyy-MM-dd");
                string timeNow = DateTime.Now.ToString("HH:mm:ss");

                finalTable = table.Copy();

                #region Column Name Change

                finalTable.Columns["PLANT_CODE"].ColumnName = "BranchCode";
                finalTable.Columns["REFERENCE_NO"].ColumnName = "Reference_No";
                finalTable.Columns["COMMENTS"].ColumnName = "Comments";
                finalTable.Columns["RETURN_ID"].ColumnName = "Return_Id";
                finalTable.Columns["POST"].ColumnName = "Post";
                finalTable.Columns["ITEM_CODE"].ColumnName = "Item_Code";
                finalTable.Columns["ITEM_NAME"].ColumnName = "Item_Name";
                finalTable.Columns["QUANTITY"].ColumnName = "Quantity";

                #endregion

                #region New Column add

                if (!finalTable.Columns.Contains("Issue_DateTime"))
                {
                    var columnName = new DataColumn("Issue_DateTime") { DefaultValue = timeNow };
                    finalTable.Columns.Add(columnName);
                }

                #endregion

                ProductDAL dal = new ProductDAL();

                foreach (DataRow rows in finalTable.Rows)
                {
                    ////if (rows["Item_Code"].ToString() == "301111" || rows["Item_Code"].ToString() == "300804")
                    ////{
                    ////    rows["UOM"] = "L";
                    ////}

                    rows["Post"] = "N";

                    if (finalTable.Columns.Contains("ISSUE_DATE") && finalTable.Columns.Contains("ISSUE_TIME"))
                    {
                        string pattern = "yyyyMMdd HHmmss";
                        DateTime parsedDate;
                        var a = rows["ISSUE_DATE"].ToString() + " " + rows["ISSUE_TIME"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["Issue_DateTime"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else if (finalTable.Columns.Contains("ISSUE_DATE"))
                    {
                        string pattern = "yyyyMMdd";
                        DateTime parsedDate;
                        var a = rows["ISSUE_DATE"].ToString();
                        DateTime.TryParseExact(a, pattern, null, DateTimeStyles.None, out parsedDate);
                        rows["Issue_DateTime"] = parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    }


                }

                finalTable.Columns.Remove("ISSUE_DATE");
                finalTable.Columns.Remove("ISSUE_TIME");

                return finalTable;

            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
