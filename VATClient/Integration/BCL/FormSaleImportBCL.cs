using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
    public partial class FormSaleImportBCL : Form
    {
        #region Variables
        string[] sqlResults = new string[6];
        string selectedTable;
        string loadedTable;
        static string selectedType;
        private int _saleRowCount = 0;
        DataTable dtTableResult = new DataTable();
        DataTable dtMaster = new DataTable();
        DataTable dtDetails = new DataTable();
        DataSet dataSet = new DataSet();

        DataSet ds;
        private bool _isDeleteTemp = true;
        public string preSelectTable;
        public string transactionType;
        private DataTable _noBranch;
        private long _timePassedInMs;
        private const string CONST_DATABASE = "Database";
        private const string CONST_TEXT = "Text";
        private const string CONST_EXCEL = "Excel";
        private const string CONST_SALETYPE = "Sales";
        List<string> postIdList = new List<string>();
        List<ErrorMessage> errormessage = new List<ErrorMessage>();

        private string _saleRow = "";

        private string invoiceNo = "";
        private bool IsExcel = false;


        private string ToDate;
        private string FromDate;
        private string InvoiceDateTime;

        bool Exists = false;

        #endregion

        public FormSaleImportBCL()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public static string SelectOne(string transactionType)
        {
            FormSaleImportBCL form = new FormSaleImportBCL();

            form.preSelectTable = "Sales";
            form.transactionType = transactionType;
            form.ShowDialog();

            return form._saleRow;
        }

        private void FormMasterImport_Load(object sender, EventArgs e)
        {
            //dgvLoadedTable.ReadOnly = true;
            //dgvLoadedTable.Columns[0].ReadOnly = false;
            tableLoad();
            typeLoad();


        }

        private void tableLoad()
        {
        }

        private void typeLoad()
        {
            cmbImportType.Items.Add(CONST_EXCEL);
            //cmbImportType.Items.Add(CONST_TEXT);
            cmbImportType.Items.Add(CONST_DATABASE);
            // cmbImportType.Items.Add(CONST_ProcessOnly);

            CommonDAL dal = new CommonDAL();

            string value = dal.settingsDesktop("Import", "SaleImportSelection",null,connVM);

            cmbImportType.SelectedItem = CONST_EXCEL;

            //cmbImportType.Text = value;
            #region IsIntegrationExcel & Other Lisence Check
            if (Program.IsIntegrationExcel == false)
            {
                cmbImportType.Items.Remove(CONST_EXCEL);
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
                
                //progressBar1.Visible = true;

                //ReadFromExcel();
                //lblRecord.Text = "Record Count: " + dtTableResult.Rows.Count;

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

        private void ReadFromExcel()
        {
            ds = LoadFromExcel();


            foreach (DataTable table in ds.Tables)
            {
                if (table.TableName == "SaleM")
                {
                    Exists = true;
                    break;
                }
            }

            if (!Exists)
            {
                throw new Exception("Desired Sheet Not Found");

            }

            dtTableResult = ds.Tables["SaleM"];

            dtTableResult =
                OrdinaryVATDesktop.DtDateCheck(dtTableResult, new string[] {"Delivery_Date_Time", "Invoice_Date_Time"});
            //dgvLoadedTable.DataSource = dtTableResult;
            loadedTable = CONST_SALETYPE;

            //OrdinaryVATDesktop.EmptyRowCheckAndDelete(dtTableResult, errormessage);
            //if (errormessage.Count > 0)
            //{
            //    FormErrorMessage.ShowDetails(errormessage);
            //    btnSaveTemp.Enabled = false;
            //}
            //errormessage.Clear();
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
            ToDate = dtpSaleToDate.Checked == false ? "3000-12-31" : dtpSaleToDate.Value.ToString("yyyy-MMM-dd");
            //DateRange = FromDate + " - " + ToDate;

        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (false)
            {
                _isDeleteTemp = true;
                LoadDataGrid();
            }
            else
            {
                try
                {
                    selectedType = cmbImportType.Text;
                    invoiceNo = txtId.Text.Trim();

                    progressBar1.Visible = true;
                    NullCheck();

                    GetSearchData();
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



        }

        private void GetSearchData(bool populateGrid = true, bool isExcelDownload = false)
        {
            ImportDAL importDal = new ImportDAL();
            BranchProfileDAL dal = new BranchProfileDAL();
            
            dataSet.Clear();
            dtTableResult.Clear();
            if (cmbImportType.Text == CONST_EXCEL)
            {
                dataSet = importDal.GetSaleNewProcData("", null, "", "", "0",isExcelDownload,connVM);
            }
            else
            {
                DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);
                dataSet = importDal.GetSaleNewProcData(invoiceNo, dt, FromDate, ToDate,"0",false,connVM);
            }

            IsExcel = cmbImportType.Text == CONST_EXCEL;


            if (dataSet != null && populateGrid)
            {
                dtMaster = dataSet.Tables[0].Copy();
                //dtDetails = dataSet.Tables[0];

                DataColumn dataColumn = new DataColumn()
                    {ColumnName = "Check", DataType = typeof(Boolean), DefaultValue = true};
                dtMaster.Columns.Add(dataColumn);
                dataColumn.SetOrdinal(0);

                dgvLoadedTable.DataSource = dtMaster;

                dgvLoadedTable.Columns["TotalValueDiff"].DefaultCellStyle.BackColor = Color.DarkRed;
                dgvLoadedTable.Columns["TotalValueDiff"].DefaultCellStyle.ForeColor = Color.GreenYellow;

                lblRecord.Text = "Record Count: " + dtMaster.Rows.Count;

                chkCheckAll.Checked = false;
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


                //fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                //BugsBD
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*|CSV files (*.csv*)|*.csv*";


                //if (cmbImportType.Text == CONST_EXCEL)
                //{
                //}
                //else
                //{
                //    fdlg.Filter = "CSV files (*.csv*)|*.csv*|Text files (*.txt*)|*.txt*";
                //}
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
            DataGridView gridView = dgvLoadedTable;
            dtTableResult = (DataTable)gridView.DataSource;

            dtTableResult = dtTableResult.Copy();
            //dtTableResult.Columns.Remove("InvoiceNo");
            dtTableResult.Columns.Remove("Customer_Code");
            dtTableResult.Columns.Remove("Customer_Name");
            dtTableResult.Columns.Remove("Invoice_Date_Time");
            dtTableResult.Columns.Remove("TotalQuantity");
            dtTableResult.Columns.Remove("TotalValue");
            dtTableResult.Columns.Remove("TotalSD");
            dtTableResult.Columns.Remove("TotalVAT");
            dtTableResult.Columns.Remove("Post");

            //dtTableResult = new DataTable();

            //List<string> idsList = new List<string>();


            //if (!chkCheckAll.Checked)
            //{
            //    for (var index = 0; index < gridData.Rows.Count; index++)
            //    {
            //        DataRow row = gridData.Rows[index];
            //        if (Convert.ToBoolean(gridView["Check", index].Value))
            //        {
            //            idsList.Add("'"+row["ID"]+"'");
            //        }
            //    }

            //    string idJoin = String.Join(",", idsList);

            //    DataRow[] selectedRows = dtDetails.Select("ID in (" + idJoin + ") and IsProcessed = 'N'");

            //    if (selectedRows.Length > 0)
            //    {
            //        dtTableResult = selectedRows.CopyToDataTable();
            //    }
            //}
            //else
            //{
            //    dtTableResult = dtDetails.Copy();
            //}

            ImportDAL importDal = new ImportDAL();

            //dtTableResult = importDal.GetSelectedData(gridData, dtDetails);

            #region Data Table Column Adjust

            //AdjustDataTableColumn();

            #endregion


            return dtTableResult.Rows.Count > 0;
        }

        protected void AdjustDataTableColumn(DataTable dt)
        {
            if (dt.Columns.Contains("IsProcessed"))
            {
                dt.Columns.Remove("IsProcessed");
            }

            if (dt.Columns.Contains("SL"))
            {
                dt.Columns.Remove("SL");
            }

            if (dt.Columns.Contains("Branch_Code"))
            {
                dt.Columns.Remove("Branch_Code");
            }

            if (dt.Columns.Contains("CreatedBy"))
            {
                dt.Columns.Remove("CreatedBy");
            }

            if (dt.Columns.Contains("ReturnId"))
            {
                dt.Columns.Remove("ReturnId");
            }

            if (dt.Columns.Contains("BOMId"))
            {
                dt.Columns.Remove("BOMId");
            }

            if (dt.Columns.Contains("TransactionType"))
            {
                dt.Columns.Remove("TransactionType");
            }

            if (dt.Columns.Contains("CreatedOn"))
            {
                dt.Columns.Remove("CreatedOn");
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
                throw ex;
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
                    string SingleSaleImport = new CommonDAL().settingsDesktop("SingleImport", "SaleImport",null,connVM);
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

            #region Sale
             if (TableName == CONST_SALETYPE)
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
                ImportDAL importDAL = new ImportDAL();

                //if (InvokeRequired)
                //    Invoke((MethodInvoker)delegate { PercentBar(4); });


                sqlResults = importDAL.SaveSaleWithSteps(dtTableResult, BulkCallBack, Program.BranchId,
                    Program.BranchCode, "", transactionType, IsExcel, InvoiceDateTime, SetSteps,"",connVM);

                Invoke((MethodInvoker)delegate { GetSearchData(); });

                BulkCallBack();
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
            if (sqlResults[0].ToLower() == "success")
            {
                DataTable table = new DataTable();
                try
                {
                    ImportDAL importDal = new ImportDAL();

                    //DataView dv = new DataView(salesData);
                    
                    //table = dv.ToTable(true, "ID");

                    string[] results = importDal.UpdateSaleTransactions(salesData, IsExcel,connVM);

                    if (results[0].ToLower() != "success")
                    {
                        string message = "These Id failed to insert to other database\n";

                        foreach (DataRow row in table.Rows)
                        {
                            message += row["ID"] + "\n";
                        }

                        FileLogger.Log("NewProcess", "UpdateOtherDB", message);

                    }
                }
                catch (Exception e)
                {
                    string message = "These Id failed to insert to other database\n";

                    foreach (DataRow row in table.Rows)
                    {
                        message += row["ID"] + "\n";
                    }

                    FileLogger.Log("NewProcess", "UpdateOtherDB", message);
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

        private void SetSteps(int steps = 4)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { PercentBar(steps); });
        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
                {
                    MessageBox.Show(this, "Import completed successfully");
                }
                else if (sqlResults[0] != null && sqlResults[0].ToLower() == "fail")
                {
                    MessageBox.Show(this, sqlResults[1]);

                }

                loadedTable = CONST_SALETYPE;

                EnableButton();

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
                InvoiceDateTime = "";
                if (dtpInvoiceDate.Checked)
                {
                    InvoiceDateTime = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd HH:mm");
                }

                GetMasterDataGrid();
                DisableButtons();
                bgwBigData.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                FileLogger.Log("NewProcess", "Post sale", exception.Message);

                MessageBox.Show(exception.Message);
            }
        }

        private void GetMasterDataGrid()
        {
            DataGridView gridView = dgvLoadedTable;
            dtTableResult = (DataTable) gridView.DataSource;

            dtTableResult = dtTableResult.Copy();
            //dtTableResult.Columns.Remove("InvoiceNo");
            dtTableResult.Columns.Remove("Customer_Code");
            dtTableResult.Columns.Remove("Customer_Name");
            dtTableResult.Columns.Remove("Invoice_Date_Time");
            dtTableResult.Columns.Remove("TotalQuantity");
            dtTableResult.Columns.Remove("TotalValue");
            dtTableResult.Columns.Remove("TotalSD");
            dtTableResult.Columns.Remove("TotalVAT");
            dtTableResult.Columns.Remove("Post");
            dtTableResult.Columns.Remove("TotalValueDiff");
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

        private void chkCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            DataTable table = (DataTable)dgvLoadedTable.DataSource;

            if (table != null && table.Columns.Contains("Check"))
            {
                table = table.Copy();

                table.Columns.Remove("Check");

                DataColumn dataColumn = new DataColumn() { ColumnName = "Check", DataType = typeof(Boolean), DefaultValue = chkCheckAll.Checked };
                table.Columns.Add(dataColumn);
                dataColumn.SetOrdinal(0);
                dgvLoadedTable.DataSource = table;
            }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Visible = true;

                //SelectInvoiceNo();

                //if (postIdList.Count == 0)
                //{
                //    MessageBox.Show("These are not in the system yet");
                //    progressBar1.Visible = false;
                //    return;

                //}

                GetMasterDataGrid();

                bgwPost.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

                FileLogger.Log("NewProcess", "Post sale", exception.Message);

            }
        }

        private void SelectInvoiceNo(bool isPrint = false)
        {
            DataGridView gridView = dgvLoadedTable;
            postIdList.Clear();
            for (var index = 0; index < gridView.RowCount; index++)
            {
                if (!isPrint)
                {
                    if (Convert.ToBoolean(gridView["Check", index].Value))
                    {
                        if (!string.IsNullOrEmpty(gridView["InvoiceNo", index].Value.ToString()))
                        {
                            postIdList.Add(gridView["InvoiceNo", index].Value.ToString());
                        }
                    }
                }
                else
                {
                    if (Convert.ToBoolean(gridView["Check", index].Value) && gridView["Post", index].Value.ToString() == "Y")
                    {
                        if (!string.IsNullOrEmpty(gridView["InvoiceNo", index].Value.ToString()))
                        {
                            postIdList.Add(gridView["InvoiceNo", index].Value.ToString());
                        }
                    }
                }

            }
        }

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaleDAL saleDal = new SaleDAL();

                //IntegrationParam integrationParam = new IntegrationParam(){IDs = postIdList};

                //SaleMasterVM SaleVM = new SaleMasterVM();
                //SaleVM.IDs = postIdList;

                ////ResultVM vm = saleDal.Multiple_SalePost(new SaleMasterVM(), integrationParam);
                //ResultVM vm = saleDal.Multiple_SalePost(SaleVM);

                ImportDAL importDal = new ImportDAL();

                sqlResults = importDal.PostSelectedData(dtTableResult,connVM);

                //sqlResults[0] = vm.Status;
                //sqlResults[1] = vm.Message;

            }
            catch (Exception exception)
            {
                sqlResults[0] = "fail";
                sqlResults[0] = exception.Message;
            }
        }

        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sqlResults[0].ToLower() == "success")
                {
                    MessageBox.Show("Selected sales have been posted");
                }
                else
                {
                    MessageBox.Show(sqlResults[1]);
                }

                GetSearchData();
            }
            catch (Exception exception)
            {
                throw;
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
                //SelectInvoiceNo();
                //if (postIdList.Count == 0)
                //{
                //    MessageBox.Show("These are not in the system yet");
                //    progressBar1.Visible = false;
                //    return;
                //}

                GetMasterDataGrid();

                SaleDAL saleDal = new SaleDAL();

                ResultVM result = saleDal.BulkInsertMasterTemp(dtTableResult,null,null,connVM);

                if (result.Status.ToLower() == "success")
                {
                    string invoices = "'5'";

                    SaleReport _reportClass = new SaleReport();

                    ReportDocument reportDocument = _reportClass.Report_VAT6_3_Completed(invoices, null, false, false, false, false, false, false
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
               

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

                FileLogger.Log("NewProcess", "Post sale", exception.Message);
            }

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            SelectInvoiceNo(true);
            if (postIdList.Count == 0) 
            {
                MessageBox.Show("These are not in the system yet");
                progressBar1.Visible = false;
                return;
            }

            CommonDAL commonDal = new CommonDAL();
            SaleReport _reportClass = new SaleReport();
            string PrinterName = commonDal.settingsDesktop("Printer", "DefaultPrinter",null,connVM);

            foreach (string invoiceNo in postIdList)
            {

                ReportDocument reportDocument = _reportClass.Report_VAT6_3_Completed(invoiceNo, null, false, false, false,
                    false, false, false
                    , false, 0, 0, false, false, false, false,false,false,connVM);


                PrinterSettings printerSettings = new PrinterSettings {PrinterName = PrinterName};
                reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);
            }

        }

        private void btnExcelUpload_Click(object sender, EventArgs e)
        {
            try
            {
                this.progressBar1.Visible = true;
                btnSaveTemp.Enabled = true;
                

                LoadDataGrid();
                

                DisableButtons();

                backgroundSaveSale.RunWorkerAsync();
                this.progressBar1.Visible = true;

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void DisableButtons()
        {
            btnExcelUpload.Enabled = false;
            btnPost.Enabled = false;
            btnPreview.Enabled = false;
            btnPrint.Enabled = false;
            btnExcelDownload.Enabled = false;
            btnSaveTemp.Enabled = false;
        }


        private void backgroundSaveSale_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaleDAL saleDal = new SaleDAL();
                ReadFromExcel();

                if (!Exists)
                {
                    return;
                }

                SaleMasterVM saleMasterVm = new SaleMasterVM()
                {
                    BranchCode = Program.BranchCode,
                    BranchId = Program.BranchId,
                    TransactionType = transactionType,
                    CurrentUser=Program.CurrentUser
                };

                sqlResults =  saleDal.ImportExcelIntegrationDataTable(dtTableResult,
                    saleMasterVm);

                DataTable table  = saleDal.GetDuplicateData(null,null,null,connVM);

                e.Result = table;

            }
            catch (Exception ex)
            {
                sqlResults[0] = "fail";
                sqlResults[1] = ex.Message;
            }
        }


        private void backgroundSaveSale_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                if (e.Result != null)
                {
                    var dataTable = (DataTable)e.Result;

                    if (dataTable.Rows.Count > 0)
                    {
                        dgvLoadedTable.DataSource = dataTable;
                        MessageBox.Show(this, "There are duplicate ID in the current data. Please provide unique ID numbers.");
                    }
                    else
                    {
                        ResultMessage();
                    }
                    
                }
                else
                {
                    ResultMessage();
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

        private void ResultMessage()
        {
            if (sqlResults[0] != null && sqlResults[0].ToLower() == "success")
            {
                MessageBox.Show(this, "Import completed successfully");

                GetSearchData();
            }
            else if (sqlResults[0] != null && sqlResults[0].ToLower() == "fail")
            {
                MessageBox.Show(this, sqlResults[1]);
            }

            EnableButton();
        }

        private void EnableButton()
        {
            btnExcelUpload.Enabled = true;
            btnPost.Enabled = true;
            btnPreview.Enabled = true;
            btnPrint.Enabled = true;
            btnExcelDownload.Enabled = true;
            btnSaveTemp.Enabled = true;

        }

        private void btnExcelDownload_Click(object sender, EventArgs e)
        {
            try
            {
                GetSearchData(false, true);

                DataTable masterSale = dataSet.Tables[0];

                OrdinaryVATDesktop.SaveExcel(masterSale, "Sale", "SaleM");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                FileLogger.Log("Excel download", "BCL", exception.Message);
            }
        }

    }
}
