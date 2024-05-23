using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using Microsoft.Office.Interop;
using SymphonySofttech.Utilities;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using System.Runtime.InteropServices;
using VATViewModel.DTOs;
using VATServer.Library;
using System.IO;
using OfficeOpenXml;
using Microsoft.Win32;

namespace VATClient
{
    public partial class FormSync : Form
    {
        #region Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        DataTable BranchResult;
        DataTable dtTableResult;
        DataTable ProductCategoryResult;
        DBSQLConnection _dbSqlConnection = new DBSQLConnection();
        BranchDAL branchDal = new BranchDAL();
        CommonDAL cDal = new CommonDAL();
        string selectedTable;
        string loadedTable;
        string selectedBranch;
        string loadedDatabase;
        string IdName = "";
        string[] result = new string[6];

        #endregion
        public FormSync()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        private void FormSync_Load(object sender, EventArgs e)
        {
            tableLoad();
            branchLoad();

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = cDal.settingsDesktop("Branch", "BranchDropDownWidth");
            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

        }

        private void tableLoad()
        {
            cmbTable.Items.Add("Product");
            cmbTable.Items.Add("Vendor");
            cmbTable.Items.Add("Customer");
            cmbTable.Items.Add("Bank");
            cmbTable.Items.Add("Vehicle");
            cmbTable.Items.Add("BOM");
            //cmbTable.Items.Add("Purchase");
            //cmbTable.Items.Add("Issue");
            //cmbTable.Items.Add("Receive");
            //cmbTable.Items.Add("Sale");
        }

        private void branchLoad()
        {
            try
            {

                progressBar1.Visible = true;
                backgroundWorkerBranch.RunWorkerAsync();

            }
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupSearch", exMessage);
            }
            #endregion

        }

        private void backgroundWorkerBranch_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try
            try
            {

                BranchResult = new DataTable();
                BranchResult = branchDal.SearchBranchNameByParam(null,null,connVM);

            }
            #endregion
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            #endregion

        }

        private void backgroundWorkerBranch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try
            try
            {
                cmbBranch.Items.Clear();
                foreach (DataRow item in BranchResult.Rows)
                {
                    cmbBranch.Items.Add(item["Name"]);
                }

            }
            #endregion
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            #endregion
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                progressBar1.Visible = true;
                #region Get Database According to Branch
                selectedBranch = cmbBranch.Text;
                selectedTable = cmbTable.Text;
                DataTable singleBranch = branchDal.SearchBranchNameByParam(selectedBranch,null,connVM);
                loadedDatabase = singleBranch.Rows[0]["DBName"].ToString();
                #endregion

                if (selectedTable == null || selectedTable == "" || selectedBranch == null || selectedBranch == "")
                {
                    this.progressBar1.Visible = false;
                    MessageBox.Show("Please select both Table and Branch !");
                    return;
                }
                dgvLoadedTable.DataSource = null;
                switch (selectedTable)
                {
                    case "Customer":
                        dtTableResult = cDal.GenericSelection("Customers", loadedDatabase,null,null,connVM);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = "Customers";
                        break;
                    case "Product":
                        if (cmbProductType.Text == "All")
                        {
                            dtTableResult = cDal.GenericSelection("Products", loadedDatabase,null,null,connVM);
                        }
                        else
                        {
                            string[] cFields = new string[] { "CategoryID" };
                            string[] cVals = new string[] { txtCategoryId.Text };
                            dtTableResult = cDal.GenericSelection("Products", loadedDatabase,cFields,cVals,connVM);
                        }
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = "Products";
                        break;
                    case "Vendor":
                        dtTableResult = cDal.GenericSelection("Vendors", loadedDatabase,null,null,connVM);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = "Vendors";
                        break;
                    case "Bank":
                        dtTableResult = cDal.GenericSelection("BankInformations", loadedDatabase,null,null,connVM);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = "BankInformations";
                        break;
                    case "Vehicle":
                        dtTableResult = cDal.GenericSelection("Vehicles", loadedDatabase,null,null,connVM);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = "Vehicles";
                        break;
                    case "BOM":
                        dtTableResult = cDal.GenericSelection("BOMs", loadedDatabase,null,null,connVM);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = "BOMs";
                        break;
                    case "Purchase":
                        dtTableResult = cDal.GenericSelectionNotSync("PurchaseInvoiceHeaders", loadedDatabase,connVM);
                        
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = "PurchaseInvoiceHeaders";
                        break;
                    case "Sale":
                        dtTableResult = cDal.GenericSelectionNotSync("SalesInvoiceHeaders", loadedDatabase,connVM);
                        dgvLoadedTable.DataSource = dtTableResult;
                        loadedTable = "SalesInvoiceHeaders";
                        break;
                    default:
                        break;
                }
            }
            #endregion
            
            #region catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerReceiveBasicSearch_DoWork", exMessage);
            }
            #endregion
            finally
            {
                progressBar1.Visible = false;
            }

        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (dtTableResult==null || dtTableResult.Rows.Count <= 0)
                {
                    this.progressBar1.Visible = false;
                    MessageBox.Show("No data to synchronize");
                    return;
                }
                string loginDatabase = DatabaseInfoVM.DatabaseName;
                if (loginDatabase == loadedDatabase)
                {
                    this.progressBar1.Visible = false;
                    MessageBox.Show("No need to synchronize with self Database!");
                    return;
                }

                switch (loadedTable)
                {
                    case "Customers":
                        IdName = "CustomerID";
                        break;
                    case "Products":
                        IdName = "ItemNo";
                        break;
                    case "Vendors":
                        IdName = "VendorID";
                        break;
                    case "BankInformations":
                        IdName = "BankID";
                        break;
                    case "Vehicles":
                        IdName = "VehicleID";
                        break;
                    default:
                        break;
                }
                progressBar1.Visible = true;
                backgroundWorkerSynchronize.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void backgroundWorkerSynchronize_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DataTable dtTableGroupResult;
                DataSet ds = new DataSet();
                switch (loadedTable)
                {
                    case "Products": 
                        dtTableGroupResult = cDal.GenericSelection("ProductCategories", loadedDatabase,null,null,connVM);
                        result = cDal.Synchronize("ProductCategories", dtTableGroupResult, "CategoryID",null,null,connVM);
                        result = cDal.Synchronize(loadedTable, dtTableResult, IdName,null,null,connVM);
                        break;
                    case "Vendors":
                        dtTableGroupResult = cDal.GenericSelection("VendorGroups", loadedDatabase,null,null,connVM);
                        result = cDal.Synchronize("VendorGroups", dtTableGroupResult, "VendorGroupID",null,null,connVM);
                        result = cDal.Synchronize(loadedTable, dtTableResult, IdName,null,null,connVM);
                        break;
                    case "Customers":
                        dtTableGroupResult = cDal.GenericSelection("CustomerGroups", loadedDatabase,null,null,connVM);
                        result = cDal.Synchronize("CustomerGroups", dtTableGroupResult, "CustomerGroupID",null,null,connVM);
                        result = cDal.Synchronize(loadedTable, dtTableResult, IdName,null,null,connVM);
                        break;
                    case "BOMs":
                        DataTable dtBomRaw = cDal.GenericSelection("BOMRaws", loadedDatabase,null,null,connVM);
                        DataTable dtCompanyOverhead = cDal.GenericSelection("BOMCompanyOverhead", loadedDatabase,null,null,connVM);
                        result = cDal.BomSynchronize(dtTableResult, dtBomRaw, dtCompanyOverhead,connVM);
                        break;
                    case "BankInformations":
                        result = cDal.Synchronize(loadedTable, dtTableResult, IdName,null,null,connVM);
                        break;
                    case "Vehicles":
                        result = cDal.Synchronize(loadedTable, dtTableResult, IdName,null,null,connVM);
                        break;
                    case "PurchaseInvoiceHeaders":
                        ds = new DataSet();
                        DataTable dtPurchaseHeader = dtTableResult;
                        DataTable dtPurchaseDetails = cDal.GenericSelectionNotSync("PurchaseInvoiceDetails", loadedDatabase,connVM);
                        dtPurchaseHeader.TableName = "PurchaseInvoiceHeaders";
                        dtPurchaseDetails.TableName = "PurchaseInvoiceDetails";
                        foreach (DataRow row in dtPurchaseHeader.Rows)
                        {
                            row["IsSynced"] = "Y";
                        }
                        foreach (DataRow row in dtPurchaseDetails.Rows)
                        {
                            row["IsSynced"] = "Y";
                        }
                        ds.Tables.Add(dtPurchaseHeader);
                        ds.Tables.Add(dtPurchaseDetails);

                        result = cDal.TransactionSync("PurchaseInvoiceHeaders",ds,loadedDatabase);
                        break;
                    case "SalesInvoiceHeaders":
                        ds = new DataSet();
                        DataTable dtSalesHeader = dtTableResult;
                        DataTable dtSaleDetails = cDal.GenericSelectionNotSync("PurchaseInvoiceDetails", loadedDatabase,connVM);
                        dtSalesHeader.TableName = "SalesInvoiceHeaders";
                        dtSaleDetails.TableName = "SalesInvoiceDetails";
                        foreach (DataRow row in dtSalesHeader.Rows)
                        {
                            row["IsSynced"] = "Y";
                        }
                        foreach (DataRow row in dtSaleDetails.Rows)
                        {
                            row["IsSynced"] = "Y";
                        }
                        ds.Tables.Add(dtSalesHeader);
                        ds.Tables.Add(dtSaleDetails);
                        result = cDal.TransactionSync("SalesInvoiceHeaders", ds,loadedDatabase,connVM);
                        break;
                    default:
                        break;
                }
                #region Old process
                
                //if (loadedTable != "BOMs")
                //{
                //    if (loadedTable == "Products")
                //    {
                //        DataTable dtTableGroupResult = cDal.GenericSelection("ProductCategories", loadedDatabase);
                //        result = cDal.Synchronize("ProductCategories", dtTableGroupResult, "VendorGroupID");
                //    }
                //    else if (loadedTable == "Vendors")
                //    {
                //        DataTable dtTableGroupResult = cDal.GenericSelection("VendorGroups", loadedDatabase);
                //        result = cDal.Synchronize("VendorGroups", dtTableGroupResult, "CategoryID");
                //    }
                //    else if (loadedTable == "Customers")
                //    {
                //        DataTable dtTableGroupResult = cDal.GenericSelection("CustomerGroups", loadedDatabase);
                //        result = cDal.Synchronize("CustomerGroups", dtTableGroupResult, "CustomerGroupID");
                //    }
                //    result = cDal.Synchronize(loadedTable, dtTableResult, IdName);
                //}
                //else
                //{
                //    DataTable dtBomRaw = cDal.GenericSelection("BOMRaws", loadedDatabase);
                //    DataTable dtCompanyOverhead = cDal.GenericSelection("BOMCompanyOverhead", loadedDatabase);
                //    result = cDal.BomSynchronize(dtTableResult, dtBomRaw, dtCompanyOverhead);
                //}
                #endregion

                if (result[0].ToLower() == "success")
                {
                    MessageBox.Show("Synchronization completed successfully");
                }
                else
                {
                    MessageBox.Show("Synchronization Failed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void backgroundWorkerSynchronize_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
        }

        private void cmbTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = cmbTable.Text;
            if (selectedTable != "Product")
            {
                cmbProductType.Visible = false;
                return;
            }
            else
            {
                bgwProductTypes.RunWorkerAsync();
            }
        }

        private void bgwProductTypes_DoWork(object sender, DoWorkEventArgs e)
        {
            ProductCategoryDAL productCategoryDal = new ProductCategoryDAL();

            string[] cValues = new string[] { "Y" };
            string[] cFields = new string[] {"ActiveStatus like"};
            ProductCategoryResult = productCategoryDal.SelectAll(0, cFields, cValues, null, null, false,"",connVM);

        }

        private void bgwProductTypes_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ProductCategoryResult.Rows.Count > 0)
            {
                var prodCategories = (from DataRow row in ProductCategoryResult.Rows
                                      select row["CategoryName"].ToString()).ToList();

                cmbProductType.Items.Clear();
                cmbProductType.Items.AddRange(prodCategories.ToArray());
                cmbProductType.Items.Insert(0, "All");
                cmbProductType.SelectedIndex = 0;
                cmbProductType.Visible = true;
            }
            
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //cmbProductType.Items.Clear();
                foreach (DataRow row in ProductCategoryResult.Rows)
                {
                    if (row[1].ToString() == cmbProductType.Text.Trim())
                    {
                        txtCategoryId.Text = row["CategoryID"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (loadedTable == "BOMs")
            {
                return;
            }
            try
            {
                DataTable dt = new DataTable();
                if (dtTableResult != null && dtTableResult.Rows.Count > 0)
                {
                    dt = dtTableResult;
                }

                string pathRoot = GetDownloadFolderPath();
                string fileDirectory = pathRoot+"//Excel Files";
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                fileDirectory += "\\"+loadedTable+".xlsx";
                FileStream objFileStrm = File.Create(fileDirectory);

                using (ExcelPackage pck = new ExcelPackage(objFileStrm))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(loadedTable);
                    ws.Cells["A1"].LoadFromDataTable(dt, true);
                    pck.Save();
                    objFileStrm.Close();
                }
                MessageBox.Show("Successfully Exported data as Products.xlsx in Excel files in Downloads Folder");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string GetDownloadFolderPath()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
        }

        
    }
}