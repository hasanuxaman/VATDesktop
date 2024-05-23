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
using Microsoft.Office.Interop.Excel;
using SymphonySofttech.Utilities;
//
//
//using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using System.Runtime.InteropServices;
using VATViewModel.DTOs;
using VATServer.Library;
using System.IO;
using Excel;
using DataTable = System.Data.DataTable;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormBranchImport : Form
    {
        DataTable dt = new DataTable();


        public FormBranchImport()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        CommonDAL _cDal = new CommonDAL();

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private bool ChangeData = false;
        public string VFIN = "1111";
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private static bool NotNumeric = false;
        private static bool empty = false;
        private static bool NotDate = false;
        private static bool NotActiveCharacter = false;
        public OleDbConnection theConnection;

        List<ProductVM> products = new List<ProductVM>();
        List<BranchProfileVM> BranchProfile = new List<BranchProfileVM>();
        List<BranchProfileVM> BranchMapDetails = new List<BranchProfileVM>();
        List<ProductVM> productDetailsvm = new List<ProductVM>();
        List<VendorVM> vendors = new List<VendorVM>();
        List<CustomerVM> customers = new List<CustomerVM>();
        List<CustomerVM> customeradd = new List<CustomerVM>();
        List<VehicleVM> vehicles = new List<VehicleVM>();
        List<BankInformationVM> banks = new List<BankInformationVM>();
        List<CostingVM> costings = new List<CostingVM>();
        List<UOMNameVM> uoms = new List<UOMNameVM>();

        private string Productresult = string.Empty;
        private string CustomerResult = string.Empty;
        private string VendorResult = string.Empty;
        private string BankResult = string.Empty;
        private string VehicleResult = string.Empty;
        private string CostingResult = string.Empty;


        private DataTable customerAddress = new DataTable();
        private DataTable productDetails = new DataTable();
        private DataTable BranchDetails = new DataTable();
        private DataTable productStock = new DataTable();



        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;

        //System.Data.DataTable dtTrack = new System.Data.DataTable();
        List<TrackingVM> trackings = new List<TrackingVM>();
        private string IsTracking = string.Empty;

        #region Header and Import

        private void IssueHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 9;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "Code";
            dataGridView1.Columns[2].Name = "ProductName";
            dataGridView1.Columns[3].Name = "UOM";
            dataGridView1.Columns[4].Name = "CostPrice";
            dataGridView1.Columns[5].Name = "VATRate";
            dataGridView1.Columns[6].Name = "Comments";
            dataGridView1.Columns[7].Name = "SDRate";
            dataGridView1.Columns[8].Name = "TradingMarkUp";
        }

        private void IssueImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + fileName + ";" +
                                          "Extended Properties=" + "\"" + "Excel 12.0;HDR=YES;" + "\"";
                theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Issue$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);

                for (int i = 2; i < dt.Rows.Count; i++)
                {
                    string[] row = new string[]
                                       {
                                           //dt.Rows[i]["Sl"].ToString(),
                                           dt.Rows[i]["Code"].ToString(),
                                           dt.Rows[i]["ProductName"].ToString(),
                                           //dt.Rows[i]["UOM"].ToString(),
                                           //dt.Rows[i]["CostPrice"].ToString(),
                                           //dt.Rows[i]["VATRate"].ToString(),
                                           //dt.Rows[i]["Comments"].ToString(),
                                           //dt.Rows[i]["SDRate"].ToString(),
                                           //dt.Rows[i]["TradingMarkUp"].ToString(),
                                       };
                    dataGridView1.Rows.Add(row);
                }
                theConnection.Close();
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "IssueImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "IssueImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "IssueImport", exMessage);
            }

            #endregion
        }

        private void ProductHeader()
        {
            string IsPharmaceutical = _cDal.settingsDesktop("Products", "IsPharmaceutical",null,connVM);

            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            if (IsPharmaceutical == "Y")
            {
                dataGridView1.ColumnCount = 24;
            }
            else
            {
                dataGridView1.ColumnCount = 22;
            }

            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "Code";
            dataGridView1.Columns[2].Name = "ProductName";
            dataGridView1.Columns[3].Name = "Description";
            dataGridView1.Columns[4].Name = "Group";
            dataGridView1.Columns[5].Name = "UOM";
            dataGridView1.Columns[6].Name = "TotalPrice";
            dataGridView1.Columns[7].Name = "OpeningQuantity";//s
            dataGridView1.Columns[8].Name = "RefNo";
            dataGridView1.Columns[9].Name = "HSCode";
            dataGridView1.Columns[10].Name = "VATRate";
            dataGridView1.Columns[11].Name = "Comments";
            dataGridView1.Columns[12].Name = "ActiveStatus";
            dataGridView1.Columns[13].Name = "SDRate";
            dataGridView1.Columns[14].Name = "PacketPrice";
            dataGridView1.Columns[15].Name = "Trading";
            dataGridView1.Columns[16].Name = "TradingMarkUp";
            dataGridView1.Columns[17].Name = "NonStock";
            //dataGridView1.Columns[17].Name = "QuantityInHand";
            dataGridView1.Columns[18].Name = "OpeningDate";
            dataGridView1.Columns[19].Name = "TollCharge";
            dataGridView1.Columns[20].Name = "NBRPrice";
            dataGridView1.Columns[21].Name = "IsConfirmed";

            if (IsPharmaceutical == "Y")
            {
                dataGridView1.Columns[22].Name = "GenericName";
                dataGridView1.Columns[23].Name = "DARNo";
            }
            
        }

        private void TradingProductHeader()
        {
            string IsPharmaceutical = _cDal.settingsDesktop("Products", "IsPharmaceutical",null,connVM);

            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            if (IsPharmaceutical == "Y")
            {
                dataGridView1.ColumnCount = 26;
            }
            else
            {
                dataGridView1.ColumnCount = 24;
            }

            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "Code";
            dataGridView1.Columns[2].Name = "ProductName";
            dataGridView1.Columns[3].Name = "Description";
            dataGridView1.Columns[4].Name = "Group";
            dataGridView1.Columns[5].Name = "UOM";
            dataGridView1.Columns[6].Name = "TotalPrice";
            dataGridView1.Columns[7].Name = "OpeningQuantity";//s
            dataGridView1.Columns[8].Name = "RefNo";
            dataGridView1.Columns[9].Name = "HSCode";
            dataGridView1.Columns[10].Name = "VATRate";
            dataGridView1.Columns[11].Name = "Comments";
            dataGridView1.Columns[12].Name = "ActiveStatus";
            dataGridView1.Columns[13].Name = "SDRate";
            dataGridView1.Columns[14].Name = "PacketPrice";
            dataGridView1.Columns[15].Name = "Trading";
            dataGridView1.Columns[16].Name = "TradingMarkUp";
            dataGridView1.Columns[17].Name = "NonStock";
            //dataGridView1.Columns[17].Name = "QuantityInHand";
            dataGridView1.Columns[18].Name = "OpeningDate";
            dataGridView1.Columns[19].Name = "TollCharge";
            dataGridView1.Columns[20].Name = "NBRPrice";
            dataGridView1.Columns[21].Name = "IsConfirmed";
            dataGridView1.Columns[22].Name = "TradingSaleVATRate";
            dataGridView1.Columns[23].Name = "TradingSaleSD";


            if (IsPharmaceutical == "Y")
            {
                dataGridView1.Columns[24].Name = "GenericName";
                dataGridView1.Columns[25].Name = "DARNo";
            }

        }

        private void ProductOpeningHeader()

        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 6;
            dataGridView1.Columns[0].Name = "ItemNo";
            dataGridView1.Columns[1].Name = "ProductName";
            dataGridView1.Columns[2].Name = "ProductCode";
            dataGridView1.Columns[3].Name = "OpeningBalance";
            dataGridView1.Columns[4].Name = "OpeningTotalCost";
            dataGridView1.Columns[5].Name = "OpeningDate";

      
        }
        private void ProductOpeningImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                //string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                //                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                //                          "Excel 12.0;HDR=YES;" + "\"";
                //OleDbConnection theConnection = new OleDbConnection(connectionString);
                //theConnection.Open();
                //OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Vehicle$]", theConnection);
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = fileName;

                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                paramVm.TableName = "ProductOpening";

                //IImport customerDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);
                //dt = customerDal.GetDataTableFromExcel(paramVm);
                dt = new ImportDAL().GetDataTableFromExcel(paramVm);
                //theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);
                    dataGridView1["ItemNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ItemNo"].ToString();
                    dataGridView1["ProductName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ProductName"].ToString();
                    dataGridView1["ProductCode", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ProductCode"].ToString();
                    dataGridView1["OpeningBalance", dataGridView1.RowCount - 1].Value = dt.Rows[i]["OpeningBalance"].ToString();
                    dataGridView1["OpeningTotalCost", dataGridView1.RowCount - 1].Value = dt.Rows[i]["OpeningTotalCost"].ToString();
                    dataGridView1["OpeningDate", dataGridView1.RowCount - 1].Value = dt.Rows[i]["OpeningDate"].ToString();



                }
               

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "VehicleImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VehicleImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "VehicleImport", exMessage);
            }

            #endregion
        }
       

        private void ProductImport()
        {
            DataSet ds = new DataSet();
            System.Data.DataTable dt = new System.Data.DataTable();
            #region try

            try
            {
                string fileName = Program.ImportFileName;


                FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                dt = ds.Tables[0];

                if (ds.Tables.Contains("ProductDetails"))
                {
                    productDetails = ds.Tables["ProductDetails"];
                }


                if (ds.Tables.Contains("ProductStock"))
                {

                    productStock = ds.Tables["ProductStock"];

                    DataView view = new DataView(productStock);

                    productStock = view.ToTable(false, "BranchCode", "BranchName", "CategoryType", "CategoryName", "ProductCode", "ProductName",
                        "StockQuantity", "StockValue", "Comments");

                }

                reader.Close();


                //string connectionString =
                //    @"Provider=Microsoft.ACE.OLEDB.12.0;"
                //                          + "Data Source=" + fileName + " ;"
                //                          + "Extended Properties=" + "\""
                //                          + "Excel 12.0;HDR=YES;" + "\"";

              
                //theConnection = new OleDbConnection(connectionString);
                //theConnection.Open();
                //OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Product$]", theConnection);
                ////DataSet theDS = new DataSet();
                ////System.Data.DataTable dt = new System.Data.DataTable();
                //theDataAdapter.Fill(dt);



                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);
                    dataGridView1["Sl", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Sl"].ToString();
                    dataGridView1["Code", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Code"].ToString();
                    dataGridView1["ProductName", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ProductName"].ToString();
                    dataGridView1["Description", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["Description"].ToString();
                    dataGridView1["Group", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Group"].ToString();
                    dataGridView1["UOM", dataGridView1.RowCount - 1].Value = dt.Rows[i]["UOM"].ToString();
                    dataGridView1["TotalPrice", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TotalPrice"].ToString();
                    dataGridView1["OpeningQuantity", dataGridView1.RowCount - 1].Value =dt.Rows[i]["OpeningQuantity"].ToString();
                    dataGridView1["RefNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["RefNo"].ToString();
                    dataGridView1["HSCode", dataGridView1.RowCount - 1].Value = dt.Rows[i]["HSCode"].ToString();
                    dataGridView1["VATRate", dataGridView1.RowCount - 1].Value = dt.Rows[i]["VATRate"].ToString();
                    dataGridView1["Comments", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Comments"].ToString();
                    dataGridView1["ActiveStatus", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ActiveStatus"].ToString();
                    dataGridView1["SDRate", dataGridView1.RowCount - 1].Value = dt.Rows[i]["SDRate"].ToString();
                    dataGridView1["PacketPrice", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["PacketPrice"].ToString();
                    dataGridView1["Trading", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Trading"].ToString();
                    dataGridView1["TradingMarkUp", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["TradingMarkUp"].ToString();
                    dataGridView1["NonStock", dataGridView1.RowCount - 1].Value = dt.Rows[i]["NonStock"].ToString();
                    dataGridView1["OpeningDate", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["OpeningDate"].ToString();
                    dataGridView1["TollCharge", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TollCharge"].ToString();
                    dataGridView1["NBRPrice", dataGridView1.RowCount - 1].Value = dt.Rows[i]["NBRPrice"].ToString();
                    dataGridView1["IsConfirmed", dataGridView1.RowCount - 1].Value = dt.Rows[i]["IsConfirmed"].ToString();

                    string IsPharmaceutical = _cDal.settingsDesktop("Products", "IsPharmaceutical",null,connVM);

                    if (IsPharmaceutical == "Y")
                    {
                        dataGridView1["GenericName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["GenericName"].ToString();
                        dataGridView1["DARNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["DARNo"].ToString();
                    }

                   

                }
                #region Tracking
               
                if (IsTracking == "Y")
                {
                    //OleDbDataAdapter adapterTrack = new OleDbDataAdapter("SELECT * FROM [Tracking$]", theConnection);
                    System.Data.DataTable dtTrack = new System.Data.DataTable();
                    ImportVM paramVm = new ImportVM();
                    paramVm.FilePath = fileName;
                    paramVm.TableName = "Tracking";

                    //IImport ImportDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);
                    //dtTrack = ImportDal.GetDataTableFromExcel(paramVm);
                    dtTrack = new ImportDAL().GetDataTableFromExcel(paramVm);

                    //////adapterTrack.Fill(dtTrack);

                    TrackingImport(dt, dtTrack);

                }

                #endregion Tracking


                var rowsCount = productDetails.Rows.Count;

                productDetailsvm.Clear();

                for (int i = 0; i < rowsCount; i++)
                {
                    var vm = new ProductVM();

                    vm.ProductCode = productDetails.Rows[i]["Code"].ToString();
                    vm.ProductName =
                        productDetails.Rows[i]["ProductName"].ToString();

                    //vm.UOM = productDetails.Rows[i]["UOM"].ToString();


                    productDetailsvm.Add(vm);
                }


                //theConnection.Close();
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ProductImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductImport", exMessage);
            }

            #endregion
        }

        private void TradingProductImport()
        {
            DataSet ds = new DataSet();
            System.Data.DataTable dt = new System.Data.DataTable();
            #region try

            try
            {
                string fileName = Program.ImportFileName;


                FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                dt = ds.Tables[0];

                if (ds.Tables.Contains("ProductDetails"))
                {
                    productDetails = ds.Tables["ProductDetails"];
                }


                if (ds.Tables.Contains("ProductStock"))
                {

                    productStock = ds.Tables["ProductStock"];

                    DataView view = new DataView(productStock);

                    productStock = view.ToTable(false, "BranchCode", "BranchName", "CategoryType", "CategoryName", "ProductCode", "ProductName",
                        "StockQuantity", "StockValue", "Comments");

                }

                reader.Close();


                //string connectionString =
                //    @"Provider=Microsoft.ACE.OLEDB.12.0;"
                //                          + "Data Source=" + fileName + " ;"
                //                          + "Extended Properties=" + "\""
                //                          + "Excel 12.0;HDR=YES;" + "\"";


                //theConnection = new OleDbConnection(connectionString);
                //theConnection.Open();
                //OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Product$]", theConnection);
                ////DataSet theDS = new DataSet();
                ////System.Data.DataTable dt = new System.Data.DataTable();
                //theDataAdapter.Fill(dt);



                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);
                    dataGridView1["Sl", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Sl"].ToString();
                    dataGridView1["Code", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Code"].ToString();
                    dataGridView1["ProductName", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ProductName"].ToString();
                    dataGridView1["Description", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["Description"].ToString();
                    dataGridView1["Group", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Group"].ToString();
                    dataGridView1["UOM", dataGridView1.RowCount - 1].Value = dt.Rows[i]["UOM"].ToString();
                    dataGridView1["TotalPrice", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TotalPrice"].ToString();
                    dataGridView1["OpeningQuantity", dataGridView1.RowCount - 1].Value = dt.Rows[i]["OpeningQuantity"].ToString();
                    dataGridView1["RefNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["RefNo"].ToString();
                    dataGridView1["HSCode", dataGridView1.RowCount - 1].Value = dt.Rows[i]["HSCode"].ToString();
                    dataGridView1["VATRate", dataGridView1.RowCount - 1].Value = dt.Rows[i]["VATRate"].ToString();
                    dataGridView1["TradingSaleVATRate", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TradingSaleVATRate"].ToString();
                    dataGridView1["TradingSaleSD", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TradingSaleSD"].ToString();
                    dataGridView1["Comments", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Comments"].ToString();
                    dataGridView1["ActiveStatus", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ActiveStatus"].ToString();
                    dataGridView1["SDRate", dataGridView1.RowCount - 1].Value = dt.Rows[i]["SDRate"].ToString();
                    dataGridView1["PacketPrice", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["PacketPrice"].ToString();
                    dataGridView1["Trading", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Trading"].ToString();
                    dataGridView1["TradingMarkUp", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["TradingMarkUp"].ToString();
                    dataGridView1["NonStock", dataGridView1.RowCount - 1].Value = dt.Rows[i]["NonStock"].ToString();
                    dataGridView1["OpeningDate", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["OpeningDate"].ToString();
                    dataGridView1["TollCharge", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TollCharge"].ToString();
                    dataGridView1["NBRPrice", dataGridView1.RowCount - 1].Value = dt.Rows[i]["NBRPrice"].ToString();
                    dataGridView1["IsConfirmed", dataGridView1.RowCount - 1].Value = dt.Rows[i]["IsConfirmed"].ToString();

                    string IsPharmaceutical = _cDal.settingsDesktop("Products", "IsPharmaceutical",null,connVM);

                    if (IsPharmaceutical == "Y")
                    {
                        dataGridView1["GenericName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["GenericName"].ToString();
                        dataGridView1["DARNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["DARNo"].ToString();
                    }



                }
                #region Tracking

                if (IsTracking == "Y")
                {
                    //OleDbDataAdapter adapterTrack = new OleDbDataAdapter("SELECT * FROM [Tracking$]", theConnection);
                    System.Data.DataTable dtTrack = new System.Data.DataTable();
                    ImportVM paramVm = new ImportVM();
                    paramVm.FilePath = fileName;
                    paramVm.TableName = "Tracking";

                    //IImport ImportDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);
                    //dtTrack = ImportDal.GetDataTableFromExcel(paramVm);
                    dtTrack = new ImportDAL().GetDataTableFromExcel(paramVm);

                    //////adapterTrack.Fill(dtTrack);

                    TrackingImport(dt, dtTrack);

                }

                #endregion Tracking


                var rowsCount = productDetails.Rows.Count;

                productDetailsvm.Clear();

                for (int i = 0; i < rowsCount; i++)
                {
                    var vm = new ProductVM();

                    vm.ProductCode = productDetails.Rows[i]["Code"].ToString();
                    vm.ProductName =
                        productDetails.Rows[i]["ProductName"].ToString();

                    //vm.UOM = productDetails.Rows[i]["UOM"].ToString();


                    productDetailsvm.Add(vm);
                }


                //theConnection.Close();
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
                FileLogger.Log(this.Name, "TradingProductImport", exMessage);
            }

            #endregion
        }

        private void VehicleImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                //string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                //                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                //                          "Excel 12.0;HDR=YES;" + "\"";
                //OleDbConnection theConnection = new OleDbConnection(connectionString);
                //theConnection.Open();
                //OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Vehicle$]", theConnection);
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = fileName;

                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                paramVm.TableName="Vehicle";

                //IImport ImportDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);
                //dt = ImportDal.GetDataTableFromExcel(paramVm);
                dt = new ImportDAL().GetDataTableFromExcel(paramVm);

                //theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);

                    dataGridView1["Sl", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Sl"].ToString();
                    dataGridView1["Code", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Code"].ToString();
                    dataGridView1["VehicleType", dataGridView1.RowCount - 1].Value = dt.Rows[i]["VehicleType"].ToString();
                    dataGridView1["VehicleNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["VehicleNo"].ToString();
                    dataGridView1["Description", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Description"].ToString();
                    dataGridView1["Comments", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Comments"].ToString();
                    dataGridView1["ActiveStatus", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ActiveStatus"].ToString();

                    //string[] row = new string[]
                    //                   {
                    //                       dt.Rows[i]["Sl"].ToString(),
                    //                       dt.Rows[i]["VehicleType"].ToString(),
                    //                       dt.Rows[i]["VehicleNo"].ToString(),
                    //                       dt.Rows[i]["Description"].ToString(),
                    //                       dt.Rows[i]["Comments"].ToString(),
                    //                       dt.Rows[i]["ActiveStatus"].ToString()
                    //                   };
                    //dataGridView1.Rows.Add(row);

                }
                //theConnection.Close();

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "VehicleImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VehicleImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "VehicleImport", exMessage);
            }

            #endregion
        }

        private void VehicleHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 7;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "Code";
            dataGridView1.Columns[2].Name = "VehicleType";
            dataGridView1.Columns[3].Name = "VehicleNo";
            dataGridView1.Columns[4].Name = "Description";
            dataGridView1.Columns[5].Name = "Comments";
            dataGridView1.Columns[6].Name = "ActiveStatus";
        }

        private void BankImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                //string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                //                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                //                          "Excel 12.0;HDR=YES;" + "\"";
                //OleDbConnection theConnection = new OleDbConnection(connectionString);
                //theConnection.Open();
                //OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Bank$]", theConnection);
                DataSet theDS = new DataSet();
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = fileName;
                System.Data.DataTable dt = new System.Data.DataTable();
                paramVm.TableName = "Bank";
                //theDataAdapter.Fill(dt);

                //IImport ImportDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);
                //dt = ImportDal.GetDataTableFromExcel(paramVm);
                dt = new ImportDAL().GetDataTableFromExcel(paramVm);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);

                    dataGridView1["Sl", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Sl"].ToString();
                    dataGridView1["Code", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Code"].ToString();
                    dataGridView1["BankName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["BankName"].ToString();
                    dataGridView1["BranchName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["BranchName"].ToString();
                    dataGridView1["AccountNumber", dataGridView1.RowCount - 1].Value = dt.Rows[i]["AccountNumber"].ToString();
                    dataGridView1["Address1", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address1"].ToString();
                    dataGridView1["Address2", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address2"].ToString();
                    dataGridView1["Address3", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address3"].ToString();
                    dataGridView1["City", dataGridView1.RowCount - 1].Value = dt.Rows[i]["City"].ToString();
                    dataGridView1["TelephoneNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TelephoneNo"].ToString();
                    dataGridView1["FaxNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["FaxNo"].ToString();
                    dataGridView1["Email", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Email"].ToString();
                    dataGridView1["ContactPerson", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ContactPerson"].ToString();
                    dataGridView1["ContactPersonDesignation", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ContactPersonDesignation"].ToString();
                    dataGridView1["ContactPersonTelephone", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ContactPersonTelephone"].ToString();
                    dataGridView1["ContactPersonEmail", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ContactPersonEmail"].ToString();
                    dataGridView1["Comments", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Comments"].ToString();
                    dataGridView1["ActiveStatus", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ActiveStatus"].ToString();
                }
                //theConnection.Close();

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "BankImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BankImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BankImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "BankImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BankImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "BankImport", exMessage);
            }

            #endregion
        }

        private void BankHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView1.ColumnCount = 18;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "Code";
            dataGridView1.Columns[2].Name = "BankName";
            dataGridView1.Columns[3].Name = "BranchName";
            dataGridView1.Columns[4].Name = "AccountNumber";
            dataGridView1.Columns[5].Name = "Address1";
            dataGridView1.Columns[6].Name = "Address2";
            dataGridView1.Columns[7].Name = "Address3";
            dataGridView1.Columns[8].Name = "City";
            dataGridView1.Columns[9].Name = "TelephoneNo";
            dataGridView1.Columns[10].Name = "FaxNo";
            dataGridView1.Columns[11].Name = "Email";
            dataGridView1.Columns[12].Name = "ContactPerson";
            dataGridView1.Columns[13].Name = "ContactPersonDesignation";
            dataGridView1.Columns[14].Name = "ContactPersonTelephone";
            dataGridView1.Columns[15].Name = "ContactPersonEmail";
            dataGridView1.Columns[16].Name = "Comments";
            dataGridView1.Columns[17].Name = "ActiveStatus";
        }

        private void OverHeadImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                          "Excel 12.0;HDR=YES;" + "\"";
                OleDbConnection theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [OverHead$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] row = new string[]
                                       {
                                           dt.Rows[i]["Sl"].ToString(),
                                           dt.Rows[i]["HeadName"].ToString(),
                                           dt.Rows[i]["Description"].ToString(),
                                           dt.Rows[i]["Comments"].ToString(),
                                           dt.Rows[i]["ActiveStatus"].ToString()
                                       };
                    dataGridView1.Rows.Add(row);
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "OverHeadImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "OverHeadImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "OverHeadImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "OverHeadImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "OverHeadImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "OverHeadImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "OverHeadImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "OverHeadImport", exMessage);
            }

            #endregion
        }

        private void OverHeadHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 6;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "HeadName";
            dataGridView1.Columns[2].Name = "Description";
            dataGridView1.Columns[3].Name = "Comments";
            dataGridView1.Columns[4].Name = "ActiveStatus";
        }

        private void ProductTypeHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "ProductType";
            dataGridView1.Columns[2].Name = "Comments";
            dataGridView1.Columns[3].Name = "Description";
            dataGridView1.Columns[4].Name = "ActiveStatus";
        }

        private void ProductTypeImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                          "Excel 12.0;HDR=YES;" + "\"";
                OleDbConnection theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [ProductType$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dataGridView1.Rows.Add(NewRow);

                    dataGridView1["Sl", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Sl"].ToString();
                    dataGridView1["ProductType", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ProductType"].ToString();
                    dataGridView1["Comments", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Comments"].ToString();
                    dataGridView1["Description", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["Description"].ToString();
                    dataGridView1["ActiveStatus", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ActiveStatus"].ToString();

                    //string[] row = new string[] 
                    //{
                    //dt.Rows[i]["Sl"].ToString(),
                    //dt.Rows[i]["ProductType"].ToString(),
                    //dt.Rows[i]["Comments"].ToString(),

                    //dt.Rows[i]["Description"].ToString(),
                    //dt.Rows[i]["ActiveStatus"].ToString()               
                    //};
                    //dataGridView1.Rows.Add(row);
                }
                //dgvRoll.Rows.Clear();

                //DataGridViewRow NewRow3 = new DataGridViewRow();
                //dgvRoll.Rows.Add(NewRow3);
                //dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                //dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Product";
                //dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Product";
                //dgvRoll["ID", dgvRoll.RowCount - 1].Value = "101";
                ////dgvRoll.Rows.Add(NewRow3);

                //DataGridViewRow NewRow4 = new DataGridViewRow();
                //dgvRoll.Rows.Add(NewRow4);
                //dgvRoll["Root", dgvRoll.RowCount - 1].Value = "Setup";
                //dgvRoll["Child", dgvRoll.RowCount - 1].Value = "Product";
                //dgvRoll["ChildChild", dgvRoll.RowCount - 1].Value = "Product Group";
                //dgvRoll["ID", dgvRoll.RowCount - 1].Value = "102";

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ProductTypeImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductTypeImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductTypeImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ProductTypeImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductTypeImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductTypeImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductTypeImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductTypeImport", exMessage);
            }

            #endregion
        }

        private void ProductCategoryHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 10;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "GroupName";
            dataGridView1.Columns[2].Name = "Description";
            dataGridView1.Columns[3].Name = "Comments";
            dataGridView1.Columns[4].Name = "Type";
            dataGridView1.Columns[5].Name = "VATRate";
            dataGridView1.Columns[6].Name = "ActiveStatus";
            dataGridView1.Columns[7].Name = "SDRate";
            dataGridView1.Columns[8].Name = "Trading";
            dataGridView1.Columns[9].Name = "NonStock";
        }

        private void ProductCategoryImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                          "Excel 12.0;HDR=YES;" + "\"";
                OleDbConnection theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [ProductGroup$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] row = new string[]
                                       {
                                           dt.Rows[i]["Sl"].ToString(),
                                           dt.Rows[i]["GroupName"].ToString(),
                                           dt.Rows[i]["Description"].ToString(),
                                           dt.Rows[i]["Comments"].ToString(),
                                           dt.Rows[i]["Type"].ToString(),
                                           dt.Rows[i]["VATRate"].ToString(),
                                           dt.Rows[i]["ActiveStatus"].ToString(),
                                           dt.Rows[i]["SDRate"].ToString(),
                                           dt.Rows[i]["Trading"].ToString(),
                                           dt.Rows[i]["NonStock"].ToString()
                                       };
                    dataGridView1.Rows.Add(row);

                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ProductCategoryImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductCategoryImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductCategoryImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ProductCategoryImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategoryImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategoryImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductCategoryImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductCategoryImport", exMessage);
            }

            #endregion
        }

        private void CustomerImport1()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                          "Excel 12.0;HDR=YES;" + "\"";
                OleDbConnection theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Customer$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] row = new string[]
                                       {
                                           dt.Rows[i]["Sl"].ToString(),
                                           dt.Rows[i]["CustomerName"].ToString(),
                                           dt.Rows[i]["CustomerGroup"].ToString(),
                                           dt.Rows[i]["Address1"].ToString(),
                                           dt.Rows[i]["Address2"].ToString(),
                                           dt.Rows[i]["Address3"].ToString(),
                                           dt.Rows[i]["City"].ToString(),
                                           dt.Rows[i]["TelephoneNo"].ToString(),
                                           dt.Rows[i]["FaxNo"].ToString(),
                                           dt.Rows[i]["Email"].ToString(),
                                           dt.Rows[i]["StartDateTime"].ToString(),
                                           dt.Rows[i]["ContactPerson"].ToString(),
                                           dt.Rows[i]["ContactPersonDesignation"].ToString(),
                                           dt.Rows[i]["ContactPersonTelephone"].ToString(),
                                           dt.Rows[i]["ContactPersonEmail"].ToString(),
                                           dt.Rows[i]["TIN"].ToString(),
                                           dt.Rows[i]["VATRegistrationNo"].ToString(),
                                           dt.Rows[i]["Comments"].ToString(),
                                           dt.Rows[i]["ActiveStatus"].ToString(),
                                           dt.Rows[i]["Country"].ToString()
                                       };
                    dataGridView1.Rows.Add(row);
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CustomerImport1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CustomerImport1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CustomerImport1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CustomerImport1", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerImport1", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerImport1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerImport1", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "CustomerImport1", exMessage);
            }

            #endregion
        }

        private void CustomerImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                //string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
                //                          + "Data Source=" + fileName + ";"
                //                          + "Extended Properties=" + "\""
                //                          + "Excel 12.0;HDR=YES;" + "\"";
                //OleDbConnection theConnection = new OleDbConnection(connectionString);
                //theConnection.Open();
                //OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Customer$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                ImportVM paramVm=new ImportVM();
                paramVm.FilePath=fileName;
                paramVm.TableName="Customer";
                //dt = new ImportDAL().GetDataTableFromExcel(paramVm);

                //IImport ImportDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);
                //var ds = ImportDal.GetDataTableFromExcelds(paramVm);
                var ds = new ImportDAL().GetDataTableFromExcelds(paramVm);

                dt = ds.Tables[paramVm.TableName];

                if (ds.Tables.Contains("CustomerAddress"))
                {
                    customerAddress = ds.Tables["CustomerAddress"];
                }

                //theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);

                    dataGridView1["Sl", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Sl"].ToString();
                    dataGridView1["Code", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Code"].ToString();
                    dataGridView1["CustomerName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["CustomerName"].ToString();
                    dataGridView1["CustomerGroup", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["CustomerGroup"].ToString();
                    dataGridView1["Address1", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address1"].ToString();
                    //dataGridView1["Address2", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address2"].ToString();
                    //dataGridView1["Address3", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address3"].ToString();
                    dataGridView1["City", dataGridView1.RowCount - 1].Value = dt.Rows[i]["City"].ToString();
                    dataGridView1["TelephoneNo", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["TelephoneNo"].ToString();
                    dataGridView1["FaxNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["FaxNo"].ToString();
                    dataGridView1["Email", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Email"].ToString();
                    dataGridView1["StartDateTime", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["StartDateTime"].ToString();
                    dataGridView1["ContactPerson", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ContactPerson"].ToString();
                    dataGridView1["ContactPersonDesignation", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ContactPersonDesignation"].ToString();
                    dataGridView1["ContactPersonTelephone", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ContactPersonTelephone"].ToString();
                    dataGridView1["ContactPersonEmail", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ContactPersonEmail"].ToString();
                    dataGridView1["TIN", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TIN"].ToString();
                    dataGridView1["VATRegistrationNo", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["VATRegistrationNo"].ToString();
                    dataGridView1["Comments", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Comments"].ToString();
                    dataGridView1["ActiveStatus", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ActiveStatus"].ToString();
                    dataGridView1["Country", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Country"].ToString();
                    dataGridView1["IsVDSWithHolder", dataGridView1.RowCount - 1].Value = dt.Rows[i]["IsVDSWithHolder"].ToString();
                    dataGridView1["IsInstitution", dataGridView1.RowCount - 1].Value = dt.Rows[i]["IsInstitution"].ToString();
                    dataGridView1["IsSpecialRate", dataGridView1.RowCount - 1].Value = dt.Rows[i]["IsSpecialRate"].ToString();
                }
                //theConnection.Close();

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CustomerImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CustomerImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CustomerImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CustomerImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "CustomerImport", exMessage);
            }

            #endregion
        }

        private void CustomerHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 24;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "Code";
            dataGridView1.Columns[2].Name = "CustomerName";
            dataGridView1.Columns[3].Name = "CustomerGroup";
            dataGridView1.Columns[4].Name = "Address1";
            //dataGridView1.Columns[5].Name = "Address2";
            //dataGridView1.Columns[6].Name = "Address3";
            dataGridView1.Columns[7].Name = "City";
            dataGridView1.Columns[8].Name = "TelephoneNo";
            dataGridView1.Columns[9].Name = "FaxNo";
            dataGridView1.Columns[10].Name = "Email";
            dataGridView1.Columns[11].Name = "StartDateTime";
            dataGridView1.Columns[12].Name = "ContactPerson";
            dataGridView1.Columns[13].Name = "ContactPersonDesignation";
            dataGridView1.Columns[14].Name = "ContactPersonTelephone";
            dataGridView1.Columns[15].Name = "ContactPersonEmail";
            dataGridView1.Columns[16].Name = "TIN";
            dataGridView1.Columns[17].Name = "VATRegistrationNo";
            dataGridView1.Columns[18].Name = "Comments";
            dataGridView1.Columns[19].Name = "ActiveStatus";
            dataGridView1.Columns[20].Name = "Country";
            dataGridView1.Columns[21].Name = "IsVDSWithHolder";
            dataGridView1.Columns[22].Name = "IsInstitution";
            dataGridView1.Columns[23].Name = "IsSpecialRate";
        }

        private void CustomerGroupImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                          "Excel 12.0;HDR=YES;" + "\"";
                OleDbConnection theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [CustomerGroup$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] row = new string[]
                                       {
                                           dt.Rows[i]["Sl"].ToString(),
                                           dt.Rows[i]["CustomerGroupName"].ToString(),
                                           dt.Rows[i]["CustomerGroupDescription"].ToString(),
                                           dt.Rows[i]["Comments"].ToString(),
                                           dt.Rows[i]["ActiveStatus"].ToString()
                                       };
                    dataGridView1.Rows.Add(row);

                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CustomerGroupImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "CustomerGroupImport", exMessage);
            }

            #endregion
        }

        private void CustomerGroupHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "CustomerGroupName";
            dataGridView1.Columns[2].Name = "CustomerGroupDescription";
            dataGridView1.Columns[3].Name = "Comments";
            dataGridView1.Columns[4].Name = "ActiveStatus";
        }

        private void VendorImport1()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                          "Excel 12.0;HDR=YES;" + "\"";
                OleDbConnection theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Vendor$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] row = new string[]
                                       {
                                           dt.Rows[i]["Sl"].ToString(),
                                           dt.Rows[i]["VendorName"].ToString(),
                                           dt.Rows[i]["VendorGroup"].ToString(),
                                           dt.Rows[i]["Address1"].ToString(),
                                           dt.Rows[i]["Address2"].ToString(),
                                           dt.Rows[i]["Address3"].ToString(),
                                           dt.Rows[i]["City"].ToString(),
                                           dt.Rows[i]["TelephoneNo"].ToString(),
                                           dt.Rows[i]["FaxNo"].ToString(),
                                           dt.Rows[i]["Email"].ToString(),
                                           dt.Rows[i]["StartDateTime"].ToString(),
                                           dt.Rows[i]["ContactPerson"].ToString(),
                                           dt.Rows[i]["ContactPersonDesignation"].ToString(),
                                           dt.Rows[i]["ContactPersonTelephone"].ToString(),
                                           dt.Rows[i]["ContactPersonEmail"].ToString(),
                                           dt.Rows[i]["VATRegistrationNo"].ToString(),
                                           dt.Rows[i]["TIN"].ToString(),
                                           dt.Rows[i]["Comments"].ToString(),
                                           dt.Rows[i]["ActiveStatus"].ToString(),
                                           dt.Rows[i]["Country"].ToString()
                                       };
                    dataGridView1.Rows.Add(row);
                }
                theConnection.Close();
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VendorImport1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VendorImport1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VendorImport1", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "VendorImport1", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorImport1", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorImport1", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorImport1", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "VendorImport1", exMessage);
            }

            #endregion
        }

        private void VendorImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                //string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                //                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                //                          "Excel 12.0;HDR=YES;" + "\"";
                //OleDbConnection theConnection = new OleDbConnection(connectionString);
                //theConnection.Open();
                //OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Vendor$]", theConnection);
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = fileName;

                DataSet theDS = new DataSet();
                paramVm.TableName = "Vendor";
                System.Data.DataTable dt = new System.Data.DataTable();

                //IImport ImportDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);
                //dt = ImportDal.GetDataTableFromExcel(paramVm);
                dt = new ImportDAL().GetDataTableFromExcel(paramVm);


                //theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);

                    dataGridView1["Sl", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Sl"].ToString();
                    dataGridView1["Code", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Code"].ToString();
                    dataGridView1["VendorName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["VendorName"].ToString();
                    dataGridView1["VendorGroup", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["VendorGroup"].ToString();
                    dataGridView1["Address1", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address1"].ToString();
                    dataGridView1["Address2", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address2"].ToString();
                    dataGridView1["Address3", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address3"].ToString();
                    dataGridView1["City", dataGridView1.RowCount - 1].Value = dt.Rows[i]["City"].ToString();
                    dataGridView1["TelephoneNo", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["TelephoneNo"].ToString();
                    dataGridView1["FaxNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["FaxNo"].ToString();
                    dataGridView1["Email", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Email"].ToString();
                    dataGridView1["StartDateTime", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["StartDateTime"].ToString();
                    dataGridView1["ContactPerson", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ContactPerson"].ToString();
                    dataGridView1["ContactPersonDesignation", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ContactPersonDesignation"].ToString();
                    dataGridView1["ContactPersonTelephone", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ContactPersonTelephone"].ToString();
                    dataGridView1["ContactPersonEmail", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ContactPersonEmail"].ToString();
                    dataGridView1["VATRegistrationNo", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["VATRegistrationNo"].ToString();
                    dataGridView1["TIN", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TIN"].ToString();
                    dataGridView1["Comments", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Comments"].ToString();
                    dataGridView1["ActiveStatus", dataGridView1.RowCount - 1].Value =
                        dt.Rows[i]["ActiveStatus"].ToString();
                    dataGridView1["Country", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Country"].ToString();
                }
                //theConnection.Close();
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VendorImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VendorImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VendorImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "VendorImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "VendorImport", exMessage);
            }

            #endregion
        }

        private void VendorHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 21;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "Code";
            dataGridView1.Columns[2].Name = "VendorName";
            dataGridView1.Columns[3].Name = "VendorGroup";
            dataGridView1.Columns[4].Name = "Address1";
            dataGridView1.Columns[5].Name = "Address2";
            dataGridView1.Columns[6].Name = "Address3";
            dataGridView1.Columns[7].Name = "City";
            dataGridView1.Columns[8].Name = "TelephoneNo";
            dataGridView1.Columns[9].Name = "FaxNo";
            dataGridView1.Columns[10].Name = "Email";
            dataGridView1.Columns[11].Name = "StartDateTime";
            dataGridView1.Columns[12].Name = "ContactPerson";
            dataGridView1.Columns[13].Name = "ContactPersonDesignation";
            dataGridView1.Columns[14].Name = "ContactPersonTelephone";
            dataGridView1.Columns[15].Name = "ContactPersonEmail";
            dataGridView1.Columns[16].Name = "VATRegistrationNo";
            dataGridView1.Columns[17].Name = "TIN";
            dataGridView1.Columns[18].Name = "Comments";
            dataGridView1.Columns[19].Name = "ActiveStatus";
            dataGridView1.Columns[20].Name = "Country";
        }

        private void VendorGroupImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                          "Excel 12.0;HDR=YES;" + "\"";
                OleDbConnection theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [VendorGroup$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] row = new string[]
                                       {
                                           dt.Rows[i]["Sl"].ToString(),
                                           dt.Rows[i]["VendorGroupName"].ToString(),
                                           dt.Rows[i]["VendorGroupDescription"].ToString(),
                                           dt.Rows[i]["Comments"].ToString(),
                                           dt.Rows[i]["ActiveStatus"].ToString()
                                       };
                    dataGridView1.Rows.Add(row);
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VendorGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VendorGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VendorGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "VendorGroupImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorGroupImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VendorGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "VendorGroupImport", exMessage);
            }

            #endregion
        }

        private void VendorGroupHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "VendorGroupName";
            dataGridView1.Columns[2].Name = "VendorGroupDescription";
            dataGridView1.Columns[3].Name = "Comments";
            dataGridView1.Columns[4].Name = "ActiveStatus";
        }

        private void BranchProfilesHeader()
        {

            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
          
           dataGridView1.ColumnCount = 20;

            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "BranchCode";
            dataGridView1.Columns[2].Name = "BranchName";
            dataGridView1.Columns[3].Name = "BranchLegalName";
            dataGridView1.Columns[4].Name = "Address";
            dataGridView1.Columns[5].Name = "City";
            dataGridView1.Columns[6].Name = "ZipCode";
            dataGridView1.Columns[7].Name = "TelephoneNo";
            dataGridView1.Columns[8].Name = "FaxNo";
            dataGridView1.Columns[9].Name = "Email";
            dataGridView1.Columns[10].Name = "ContactPerson";
            dataGridView1.Columns[11].Name = "ContactPersonDesignation";
            dataGridView1.Columns[12].Name = "ContactPersonTelephone";
            dataGridView1.Columns[13].Name = "ContactPersonEmail";
            dataGridView1.Columns[14].Name = "VatRegistrationNo";
            dataGridView1.Columns[15].Name = "BIN";
            dataGridView1.Columns[16].Name = "TINNo";
            dataGridView1.Columns[17].Name = "Comments";
            dataGridView1.Columns[18].Name = "ActiveStatus";
            dataGridView1.Columns[19].Name = "IsCentral";
            
        }

        private void BranchProfilesImport()
        {
            DataSet ds = new DataSet();
            System.Data.DataTable dt = new System.Data.DataTable();
            #region try

            try
            {
                string fileName = Program.ImportFileName;


                FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                dt = ds.Tables[0];

                if (ds.Tables.Contains("BranchMapDetails"))
                {
                    BranchDetails = ds.Tables["BranchMapDetails"];
                    foreach (DataRow dtRow in BranchDetails.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(dtRow["IntegrationCode"].ToString()))
                        {
                            throw new Exception("IntegrationCode field is empty.");                          
                        }
                        if (string.IsNullOrWhiteSpace(dtRow["BranchName"].ToString()))
                        {
                            throw new Exception("BranchName field is empty.");
                        }
                        if (string.IsNullOrWhiteSpace(dtRow["Address"].ToString()))
                        {
                            throw new Exception("Address field is empty.");
                        }

                    }
                                  
                }
                reader.Close();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);
                    dataGridView1["Sl", dataGridView1.RowCount - 1].Value = i+1;
                    dataGridView1["BranchCode", dataGridView1.RowCount - 1].Value = dt.Rows[i]["BranchCode"].ToString();
                    dataGridView1["BranchName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["BranchName"].ToString();
                    dataGridView1["BranchLegalName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["BranchLegalName"].ToString();
                    dataGridView1["Address", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Address"].ToString();
                    dataGridView1["City", dataGridView1.RowCount - 1].Value = dt.Rows[i]["City"].ToString();
                    dataGridView1["ZipCode", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ZipCode"].ToString();
                    dataGridView1["TelephoneNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TelephoneNo"].ToString();
                    dataGridView1["FaxNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["FaxNo"].ToString();
                    dataGridView1["Email", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Email"].ToString();
                    dataGridView1["ContactPerson", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ContactPerson"].ToString();
                    dataGridView1["ContactPersonDesignation", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ContactPersonDesignation"].ToString();
                    dataGridView1["ContactPersonTelephone", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ContactPersonTelephone"].ToString();
                    dataGridView1["ContactPersonEmail", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ContactPersonEmail"].ToString();
                    dataGridView1["VatRegistrationNo", dataGridView1.RowCount - 1].Value =dt.Rows[i]["VatRegistrationNo"].ToString();
                    dataGridView1["BIN", dataGridView1.RowCount - 1].Value = dt.Rows[i]["BIN"].ToString();
                    dataGridView1["TINNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["TINNo"].ToString();
                    dataGridView1["Comments", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Comments"].ToString();
                    dataGridView1["ActiveStatus", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ActiveStatus"].ToString();
                    dataGridView1["IsCentral", dataGridView1.RowCount - 1].Value = dt.Rows[i]["IsCentral"].ToString();
                   

                }

                var rowsCount = BranchDetails.Rows.Count;

                BranchMapDetails.Clear();

                for (int i = 0; i < rowsCount; i++)
                {
                    var vm = new BranchProfileVM();

                    vm.IntegrationCode = BranchDetails.Rows[i]["IntegrationCode"].ToString();
                    vm.BranchCode = BranchDetails.Rows[i]["BranchCode"].ToString();
                    vm.DetailsAddress = BranchDetails.Rows[i]["Address"].ToString();

                    //vm.UOM = productDetails.Rows[i]["UOM"].ToString();


                    BranchMapDetails.Add(vm);
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
                FileLogger.Log(this.Name, "BranchProfilesImport", exMessage);
            }

            #endregion
        }

        private void UserInformationtHeader()
        {

            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 3;

          //  dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[0].Name = "UserName";
            dataGridView1.Columns[1].Name = "BranchCode";
            dataGridView1.Columns[2].Name = "BranchName";
            //dataGridView1.Columns[3].Name = "BranchLegalName";
            //dataGridView1.Columns[4].Name = "Address";
            

        }

        private void UserInformationImport()
        {
            DataSet ds = new DataSet();
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                
                DataSet theDS = new DataSet();
                ImportVM paramVm = new ImportVM();
                paramVm.FilePath = fileName;
                paramVm.TableName = "UserInformation";

                //IImport importDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);
                //dt = importDal.GetDataTableFromExcel(paramVm);
                dt = new ImportDAL().GetDataTableFromExcel(paramVm);

                //theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);

                   // dataGridView1["User Id", dataGridView1.RowCount - 1].Value = dt.Rows[i]["UserID"].ToString();
                    dataGridView1["UserName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["UserName"].ToString();
                    dataGridView1["BranchCode", dataGridView1.RowCount - 1].Value = dt.Rows[i]["BranchCode"].ToString();
                    dataGridView1["BranchName", dataGridView1.RowCount - 1].Value = dt.Rows[i]["BranchName"].ToString();
                   // dataGridView1["Active Status", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ActiveStatus"].ToString();
                }
                //theConnection.Close();

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
                FileLogger.Log(this.Name, "UserInformationImport", exMessage);
            }

            #endregion
        }
  

        #endregion

        #region Save Import Data

        #region Block
        //private void ProductTypeSave()
        //{

        //    string ProductTypeData = string.Empty;
        //    try
        //    {

        //        for (int i = 0; i < dataGridView1.RowCount; i++)
        //        {

        //            if (ProductTypeData == string.Empty)
        //            {
        //                ProductTypeData =
        //                    "" + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["ProductType"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Comments"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() + FieldDelimeter +
        //                    Program.CurrentUser + FieldDelimeter + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") +
        //                    FieldDelimeter +
        //                    Program.CurrentUser + FieldDelimeter + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") +
        //                    FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Description"].Value.ToString() + FieldDelimeter +
        //                    "-" + FieldDelimeter + "-" + FieldDelimeter + "-" + FieldDelimeter + "-";

        //            }
        //            else
        //            {
        //                ProductTypeData = ProductTypeData + "" + FieldDelimeter +
        //                                  dataGridView1.Rows[i].Cells["ProductType"].Value.ToString() +
        //                                  FieldDelimeter +
        //                                  dataGridView1.Rows[i].Cells["Comments"].Value.ToString() +
        //                                  FieldDelimeter +
        //                                  dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() +
        //                                  FieldDelimeter + Program.CurrentUser + FieldDelimeter +
        //                                  DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") + FieldDelimeter +
        //                                  Program.CurrentUser + FieldDelimeter +
        //                                  DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") + FieldDelimeter +
        //                                  dataGridView1.Rows[i].Cells["Description"].Value.ToString() +
        //                                  FieldDelimeter + "-" + FieldDelimeter + "-" + FieldDelimeter +
        //                                  "-" + FieldDelimeter + "-";
        //            }
        //            ProductTypeData = ProductTypeData + LineDelimeter;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    try
        //    {
        //        string encriptedProductTypeDataData = Converter.DESEncrypt(PassPhrase, EnKey, ProductTypeData);
        //        ImportSoapClient ImportproductType = new ImportSoapClient();

        //        //PurchaseSoapClient PurchaseInvoiceEntry = new PurchaseSoapClient();

        //        string PurchaseInvoiceresult =
        //            ImportproductType.ImportProductType(encriptedProductTypeDataData.ToString(), Program.DatabaseName);

        //        if (PurchaseInvoiceresult == "-266")
        //        {
        //            MessageBox.Show("Data already saved", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //        else if (Convert.ToDecimal(PurchaseInvoiceresult) < 0)
        //        {
        //            MessageBox.Show("Data not save", "Product Type", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //        else
        //        {
        //            MessageBox.Show("Data save successfully", "Product Type", MessageBoxButtons.OK,
        //                            MessageBoxIcon.Information);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //}
        //private void ProductCategorySave()
        //{

        //    string ProductCategoryData = string.Empty;
        //    try
        //    {

        //        for (int i = 0; i < dataGridView1.RowCount; i++)
        //        {

        //            if (ProductCategoryData == string.Empty)
        //            {
        //                ProductCategoryData =
        //                    "" + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["GroupName"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Description"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Comments"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Type"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["VATRate"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["SDRate"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Trading"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["NonStock"].Value.ToString() + FieldDelimeter +
        //                    Program.CurrentUser + FieldDelimeter + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

        //            }
        //            else
        //            {
        //                ProductCategoryData = ProductCategoryData +
        //                                      "" + FieldDelimeter +
        //                                      dataGridView1.Rows[i].Cells["GroupName"].Value.ToString() + FieldDelimeter +
        //                                      dataGridView1.Rows[i].Cells["Description"].Value.ToString() +
        //                                      FieldDelimeter +
        //                                      dataGridView1.Rows[i].Cells["Comments"].Value.ToString() + FieldDelimeter +
        //                                      dataGridView1.Rows[i].Cells["Type"].Value.ToString() + FieldDelimeter +
        //                                      dataGridView1.Rows[i].Cells["VATRate"].Value.ToString() + FieldDelimeter +
        //                                      dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() +
        //                                      FieldDelimeter +
        //                                      dataGridView1.Rows[i].Cells["SDRate"].Value.ToString() + FieldDelimeter +
        //                                      dataGridView1.Rows[i].Cells["Trading"].Value.ToString() + FieldDelimeter +
        //                                      dataGridView1.Rows[i].Cells["NonStock"].Value.ToString() + FieldDelimeter +
        //                                      Program.CurrentUser + FieldDelimeter +
        //                                      DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
        //            }
        //            ProductCategoryData = ProductCategoryData + LineDelimeter;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    try
        //    {
        //        string encriptedProductCategoryDataData = Converter.DESEncrypt(PassPhrase, EnKey, ProductCategoryData);
        //        ImportSoapClient ImportproductCategory = new ImportSoapClient();

        //        //PurchaseSoapClient PurchaseInvoiceEntry = new PurchaseSoapClient();

        //        string productCategoryresult =
        //            ImportproductCategory.ImportProductCategory(encriptedProductCategoryDataData.ToString(),
        //                                                        Program.DatabaseName);

        //        if (productCategoryresult == "-266")
        //        {
        //            MessageBox.Show("Data already saved", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //        else if (Convert.ToDecimal(productCategoryresult) < 0)
        //        {
        //            MessageBox.Show("Data not save", "Product Cateegory", MessageBoxButtons.OK,
        //                            MessageBoxIcon.Information);
        //            return;
        //        }
        //        else
        //        {
        //            MessageBox.Show("Data save successfully", "Product Cateegory", MessageBoxButtons.OK,
        //                            MessageBoxIcon.Information);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //}
        //private void CustomerGroupSave()
        //{

        //    string CustomerGroupData = string.Empty;
        //    try
        //    {

        //        for (int i = 0; i < dataGridView1.RowCount; i++)
        //        {

        //            if (CustomerGroupData == string.Empty)
        //            {
        //                CustomerGroupData =
        //                    "" + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["CustomerGroupName"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["CustomerGroupDescription"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Comments"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() + FieldDelimeter +
        //                    Program.CurrentUser + FieldDelimeter + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

        //            }
        //            else
        //            {
        //                CustomerGroupData = CustomerGroupData +
        //                                    "" + FieldDelimeter +
        //                                    dataGridView1.Rows[i].Cells["CustomerGroupName"].Value.ToString() +
        //                                    FieldDelimeter +
        //                                    dataGridView1.Rows[i].Cells["CustomerGroupDescription"].Value.ToString() +
        //                                    FieldDelimeter +
        //                                    dataGridView1.Rows[i].Cells["Comments"].Value.ToString() + FieldDelimeter +
        //                                    dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() +
        //                                    FieldDelimeter +
        //                                    Program.CurrentUser + FieldDelimeter +
        //                                    DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
        //            }
        //            CustomerGroupData = CustomerGroupData + LineDelimeter;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    try
        //    {
        //        string encriptedCustomerGroupData = Converter.DESEncrypt(PassPhrase, EnKey, CustomerGroupData);
        //        ImportSoapClient ImportCustomerGroup = new ImportSoapClient();

        //        //PurchaseSoapClient PurchaseInvoiceEntry = new PurchaseSoapClient();

        //        string PurchaseInvoiceresult =
        //            ImportCustomerGroup.ImportCustomerGroup(encriptedCustomerGroupData.ToString(), Program.DatabaseName);

        //        if (PurchaseInvoiceresult == "-266")
        //        {
        //            MessageBox.Show("Data already saved", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //        else if (Convert.ToDecimal(PurchaseInvoiceresult) < 0)
        //        {
        //            MessageBox.Show("Data not save", "Customer Group", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //        else
        //        {
        //            MessageBox.Show("Data save successfully", "Customer Group", MessageBoxButtons.OK,
        //                            MessageBoxIcon.Information);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //}
        //private void VendorGroupSave()
        //{

        //    string VendorGroupData = string.Empty;
        //    try
        //    {

        //        for (int i = 0; i < dataGridView1.RowCount; i++)
        //        {

        //            if (VendorGroupData == string.Empty)
        //            {
        //                VendorGroupData =
        //                    "" + FieldDelimeter +

        //                    dataGridView1.Rows[i].Cells["VendorGroupName"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["VendorGroupDescription"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Comments"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() + FieldDelimeter +
        //                    Program.CurrentUser + FieldDelimeter + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

        //            }
        //            else
        //            {
        //                VendorGroupData = VendorGroupData +
        //                                  "" + FieldDelimeter +
        //                                  dataGridView1.Rows[i].Cells["VendorGroupName"].Value.ToString() +
        //                                  FieldDelimeter +
        //                                  dataGridView1.Rows[i].Cells["VendorGroupDescription"].Value.ToString() +
        //                                  FieldDelimeter +
        //                                  dataGridView1.Rows[i].Cells["Comments"].Value.ToString() + FieldDelimeter +
        //                                  dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() + FieldDelimeter +
        //                                  Program.CurrentUser + FieldDelimeter +
        //                                  DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
        //            }
        //            VendorGroupData = VendorGroupData + LineDelimeter;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    try
        //    {
        //        string encriptedVendorGroupData = Converter.DESEncrypt(PassPhrase, EnKey, VendorGroupData);
        //        ImportSoapClient ImportVendorGroup = new ImportSoapClient();

        //        //PurchaseSoapClient PurchaseInvoiceEntry = new PurchaseSoapClient();

        //        string VendorGroupResult = ImportVendorGroup.ImportVendorGroup(encriptedVendorGroupData.ToString(),
        //                                                                       Program.DatabaseName);

        //        if (VendorGroupResult == "-266")
        //        {
        //            MessageBox.Show("Data already saved", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //        else if (Convert.ToDecimal(VendorGroupResult) < 0)
        //        {
        //            MessageBox.Show("Data not save", "Vendor Group", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //        else
        //        {
        //            MessageBox.Show("Data save successfully", "Vendor Group", MessageBoxButtons.OK,
        //                            MessageBoxIcon.Information);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //}
        //private void OverHeadSave()
        //{

        //    string OverHeadData = string.Empty;
        //    try
        //    {

        //        for (int i = 0; i < dataGridView1.RowCount; i++)
        //        {

        //            if (OverHeadData == string.Empty)
        //            {
        //                OverHeadData =
        //                    "" + FieldDelimeter +

        //                    dataGridView1.Rows[i].Cells["HeadName"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Description"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["Comments"].Value.ToString() + FieldDelimeter +
        //                    dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() + FieldDelimeter +
        //                    Program.CurrentUser + FieldDelimeter + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

        //            }
        //            else
        //            {
        //                OverHeadData = OverHeadData +
        //                               "" + FieldDelimeter +
        //                               dataGridView1.Rows[i].Cells["HeadName"].Value.ToString() + FieldDelimeter +
        //                               dataGridView1.Rows[i].Cells["Description"].Value.ToString() + FieldDelimeter +
        //                               dataGridView1.Rows[i].Cells["Comments"].Value.ToString() + FieldDelimeter +
        //                               dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString() + FieldDelimeter +
        //                               Program.CurrentUser + FieldDelimeter +
        //                               DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
        //            }
        //            OverHeadData = OverHeadData + LineDelimeter;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    try
        //    {
        //        string encriptedOverHeadData = Converter.DESEncrypt(PassPhrase, EnKey, OverHeadData);
        //        ImportSoapClient ImportOverHead = new ImportSoapClient();

        //        //PurchaseSoapClient PurchaseInvoiceEntry = new PurchaseSoapClient();

        //        string OverHeadResult = ImportOverHead.ImportoverHead(encriptedOverHeadData.ToString(),
        //                                                              Program.DatabaseName);

        //        if (OverHeadResult == "-266")
        //        {
        //            MessageBox.Show("Data already saved", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //        else if (Convert.ToDecimal(OverHeadResult) < 0)
        //        {
        //            MessageBox.Show("Data not save", "Over Head", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //        else
        //        {
        //            MessageBox.Show("Data save successfully", "Over Head", MessageBoxButtons.OK,
        //                            MessageBoxIcon.Information);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.GetExceptionCode() == -532459699)
        //        {
        //            MessageBox.Show("Communication not performed" + "\n\n" + "Please contact with Administrator.",
        //                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //        MessageBox.Show(ex.Message + "\n\n" + "Please contact with Administrator.", this.Text,
        //                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //}
        #endregion Block


        private void UserInformationSave()
        {
            ////System.Data.DataTable dt = new System.Data.DataTable();
            sqlResults = new string[2];
            this.progressBar1.Visible = true;
            
            try
            {
                UserBranchDetailDAL dal = new UserBranchDetailDAL();


                string CreatedBy = Program.CurrentUser;
                string Createdon = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                sqlResults = dal.InsertfromExcel(dt, null, null, CreatedBy, Createdon, connVM);

                if (sqlResults[0] == "Success")
                {
                    MessageBox.Show("Data Import successfully", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //dtUserBranchDetailResult=dal.SelectAll()
                }
                this.progressBar1.Visible = false;
                              
               
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
                FileLogger.Log(this.Name, "ImportFromExcelFile", exMessage);
            }
           
            #endregion

        }

        #endregion

        private void cmbLoad()
        {
            cmbImport.Items.Add("UserInformation");
            //cmbImport.Items.Add("Bank"); //5
            //cmbImport.Items.Add("Customer"); // 2

        }

        //possible test code no server side
        private void button2_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                string connectionString = "Data Source=.;Initial Catalog=pubs;Integrated Security=True";
                string sql = "SELECT * FROM Authors";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlDataAdapter dataadapter = new SqlDataAdapter(sql, connection);
                DataSet ds = new DataSet();
                connection.Open();
                dataadapter.Fill(ds, "Authors_table");
                connection.Close();
                dataGridView1.DataSource = ds;
                dataGridView1.DataMember = "Authors_table";
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "button2_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }

            #endregion
        }

        //possible test code no server side
        private void button4_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                if (cmbImport.Text == "Product")
                {
                    for (int j = 0; j < dataGridView1.RowCount; j++)
                    {
                        string a = dataGridView1.Rows[j].Cells[1].Value.ToString();
                        int k = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            if (dataGridView1.Rows[i].Cells["ProductName"].Value.ToString() + "-" +
                                dataGridView1.Rows[i].Cells["RefNo"].Value.ToString() ==
                                dataGridView1.Rows[j].Cells["ProductName"].Value.ToString() + "-" +
                                dataGridView1.Rows[j].Cells["RefNo"].Value.ToString())
                            {
                                k = k + 1;
                                if (k > 1)
                                {
                                    MessageBox.Show(
                                        "Sl No: " + dataGridView1.Rows[j].Cells[0].Value.ToString() + "-" +
                                        dataGridView1.Rows[i].Cells[0].Value.ToString() + " , Name: " +
                                        dataGridView1.Rows[j].Cells[1].Value.ToString(), this.Text);
                                    dataGridView1.Rows[j].Cells["ProductName"].Value =
                                        dataGridView1.Rows[j].Cells["ProductName"].Value + "-" +
                                        dataGridView1.Rows[j].Cells["RefNo"].Value;
                                    dataGridView1.Rows[i].Cells["ProductName"].Value =
                                        dataGridView1.Rows[i].Cells["ProductName"].Value + "-" +
                                        dataGridView1.Rows[i].Cells["RefNo"].Value;

                                    //return;
                                }
                            }
                        }
                    }

                }

                else if (cmbImport.Text == "Vendor")
                {
                    for (int j = 0; j < dataGridView1.RowCount; j++)
                    {
                        string a = dataGridView1.Rows[j].Cells[1].Value.ToString();
                        int k = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {

                            if (dataGridView1.Rows[i].Cells[1].Value.ToString() ==
                                dataGridView1.Rows[j].Cells[1].Value.ToString())
                            {
                                k = k + 1;
                                if (k > 1)
                                {
                                    MessageBox.Show(
                                        "Sl No: " + dataGridView1.Rows[j].Cells[0].Value.ToString() + "-" +
                                        dataGridView1.Rows[i].Cells[0].Value.ToString() + " , Name: " +
                                        dataGridView1.Rows[j].Cells[1].Value.ToString(), this.Text);
                                    dataGridView1.Rows[j].Cells["VendorName"].Value =
                                        dataGridView1.Rows[j].Cells["VendorName"].Value + "-" +
                                        dataGridView1.Rows[j].Cells["City"].Value;
                                    dataGridView1.Rows[i].Cells["VendorName"].Value =
                                        dataGridView1.Rows[i].Cells["VendorName"].Value + "-" +
                                        dataGridView1.Rows[i].Cells["City"].Value;

                                    //return;
                                }
                            }
                        }
                    }
                }
                else if (cmbImport.Text == "Customer")
                {
                    for (int j = 0; j < dataGridView1.RowCount; j++)
                    {
                        string a = dataGridView1.Rows[j].Cells[1].Value.ToString();
                        int k = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {

                            if (dataGridView1.Rows[i].Cells[1].Value.ToString() ==
                                dataGridView1.Rows[j].Cells[1].Value.ToString())
                            {
                                k = k + 1;
                                if (k > 1)
                                {
                                    MessageBox.Show(
                                        "Sl No: " + dataGridView1.Rows[j].Cells[0].Value.ToString() + "-" +
                                        dataGridView1.Rows[i].Cells[0].Value.ToString() + " , Name: " +
                                        dataGridView1.Rows[j].Cells[1].Value.ToString(), this.Text);
                                    dataGridView1.Rows[j].Cells["CustomerName"].Value =
                                        dataGridView1.Rows[j].Cells["CustomerName"].Value + "-" +
                                        dataGridView1.Rows[j].Cells["CustomerGroup"].Value;
                                    dataGridView1.Rows[i].Cells["CustomerName"].Value =
                                        dataGridView1.Rows[i].Cells["CustomerName"].Value + "-" +
                                        dataGridView1.Rows[i].Cells["CustomerGroup"].Value;

                                    //return;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "button4_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button4_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button4_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "button4_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button4_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button4_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button4_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button4_Click", exMessage);
            }

            #endregion
        }

        private void releaseObject(object obj)
        {
            #region try

            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "releaseObject", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "releaseObject", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "releaseObject", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "releaseObject", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "releaseObject", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "releaseObject", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "releaseObject", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "releaseObject", exMessage);
            }
            #endregion

            finally
            {
                GC.Collect();
            }
        }

        //possible test code no server side
        private void button5_Click(object sender, EventArgs e)
        {
            if (chkSame.Checked != true)
            {
                BrowsFile();
            }
            IssueHeader();
            IssueImport();


        }

        private void FormImport_Load(object sender, EventArgs e)
        {
            CommonDAL commonDal = new CommonDAL();
            IsTracking = commonDal.settingsDesktop("TrackingTrace", "Tracking");
            if (string.IsNullOrEmpty(IsTracking))
            {
                MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                return;
            }
            if (IsTracking=="Y")
            {
                chkTrack.Visible = true;
            }
            else
            {
                chkTrack.Visible = false;
            }
            dgvSerialTrack.Visible = false;

            cmbLoad();
            cmbImport.SelectedIndex = 0;
            //cmbImport.SelectedText = "Select";
            ChangeData = false;
            Program.ImportFileName = "";
        }

        private bool NotEmptyCheck()
        {
            #region try

            try
            {
                //empty = false;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Value.ToString() == "")
                        {
                            MessageBox.Show("Empty field not allow.", this.Text);
                            empty = true;
                        }
                    }
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "NotEmptyCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "NotEmptyCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "NotEmptyCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "NotEmptyCheck", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NotEmptyCheck", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NotEmptyCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NotEmptyCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "NotEmptyCheck", exMessage);
            }

            #endregion

            return empty;
        }

        private bool NotNumericCheck(string column)
        {
            #region try

            try
            {
                //NotNumeric = false;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (Program.IsNumeric(dataGridView1.Rows[i].Cells[column].Value.ToString()) == false)
                    {
                        MessageBox.Show("All field need numeric value on " + column, this.Text);
                        NotNumeric = true;
                        break;
                    }
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "NotNumericCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "NotNumericCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "NotNumericCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "NotNumericCheck", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NotNumericCheck", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NotNumericCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NotNumericCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "NotNumericCheck", exMessage);
            }

            #endregion

            return NotNumeric;
        }

        private bool NotDateCheck(string column)
        {
            #region try

            try
            {
                //NotDate = false;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (Program.IsDate(dataGridView1.Rows[i].Cells[column].Value.ToString()) == false)
                    {
                        MessageBox.Show("All field need date value on " + column, this.Text);
                        NotDate = true;
                        break;
                    }
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "NotDateCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "NotDateCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "NotDateCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "NotDateCheck", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NotDateCheck", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NotDateCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NotDateCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "NotDateCheck", exMessage);
            }

            #endregion

            return NotDate;
        }

        private bool ActiveCharacterCheck(string column)
        {
            #region try

            try
            {
                //NotActiveCharacter = false;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (Program.IsActive(dataGridView1.Rows[i].Cells[column].Value.ToString()) == false)
                    {
                        MessageBox.Show("Field only Y/N value on " + column, this.Text);
                        NotActiveCharacter = true;
                        break;
                    }
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ActiveCharacterCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ActiveCharacterCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ActiveCharacterCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ActiveCharacterCheck", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ActiveCharacterCheck", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ActiveCharacterCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ActiveCharacterCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ActiveCharacterCheck", exMessage);
            }

            #endregion

            return NotActiveCharacter;
        }

        //no server Side
        private void btnImport_Click(object sender, EventArgs e)
        {

            #region try

            try
            {
                btnImport.Enabled = false;
                progressBar1.Visible = true;
                NotNumeric = false;
                empty = false;
                NotDate = false;
                NotActiveCharacter = false;

                if (cmbImport.Text != string.Empty)
                {
                    if (chkSame.Checked == false)
                    {
                        BrowsFile();
                    }
                    string fileName = Program.ImportFileName;
                    if (string.IsNullOrEmpty(fileName))
                    {
                        MessageBox.Show("Please select the right file for import");
                        return;
                    }

                    ImportFromExcelFile();
                    
                }
                else
                {
                    #region Checking For Empty Combo Box

                    MessageBox.Show("Select an item in Combo Box !", "No Item Selected In The Combo Box !",
                                    MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    #endregion
                }

                if (dataGridView1.Rows.Count > 0)
                {
                    chkSame.Checked = true;
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnImport_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnImport_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnImport_Click", exMessage);
            }

            #endregion
            finally
            {
                btnImport.Enabled =true;
                progressBar1.Visible = false;

                lbCount.Text = "Total Record(s): " + dataGridView1.Rows.Count.ToString();

            }

        }

        private void ImportFromExcelFile()
        {
            #region try

            try
            {
                if (cmbImport.Text == "UserInformation")
                {
                    #region Action cmbImport.Text == "UserInformation"

                    UserInformationtHeader();
                    UserInformationImport();

                    #endregion
                }        
              
                //else if (cmbImport.Text == "TradingProducts")
                //{
                //    #region Action cmbImport.Text == "TradingProducts"

                //    TradingProductHeader();
                //    TradingProductImport();

                //    NotNumericCheck("TotalPrice");
                //    NotNumericCheck("OpeningQuantity");
                //    NotNumericCheck("VATRate");
                //    ActiveCharacterCheck("ActiveStatus");
                //    NotNumericCheck("SDRate");
                //    NotNumericCheck("PacketPrice");
                //    ActiveCharacterCheck("Trading");
                //    NotNumericCheck("TradingMarkUp");
                //    ActiveCharacterCheck("NonStock");
                //    //NotNumericCheck("QuantityInHand");
                //    NotDateCheck("OpeningDate");
                //    NotNumericCheck("TollCharge");

                //    #endregion
                //}
               
                
                else
                {
                    MessageBox.Show("Item Management Not Done !");
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
                FileLogger.Log(this.Name, "ImportFromExcelFile", exMessage);
            }
           
            #endregion
        }

        //no server side
        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }

        private void ExportExcel()
        {
            #region try

            try
            {
                //href="http://dotnetask.com/Resource.aspx?Resourceid=644"

                #region Checking For Empty Combo Box

                if (cmbImport.SelectedIndex < 0)
                {
                    MessageBox.Show("Select an item in Combo Box !", "No Item Selected In The Combo Box !",
                                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    cmbImport.DroppedDown = true;
                }
                //if (string.IsNullOrWhiteSpace(cmbImport.Text))
                //{
                //    MessageBox.Show("Select an item in Combo Box !", "No Item Selected In The Combo Box !",
                //                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    cmbImport.DroppedDown = true;
                //}
                #endregion

                #region "Checking for Data Imported to Grid / Empty Grid Loaded"

                else if (dataGridView1.Columns.Count == 0 && dataGridView1.Rows.Count == 0)
                {
                    string msgBoxTitle = "Data Grid Not Populated !";

                    string msgTxt = "Do you want to export only heading?";
                    //"Please Import Data Before Export to Excel.";

                    DialogResult dlgResult = MessageBox.Show(msgTxt, msgBoxTitle, MessageBoxButtons.YesNo,
                                                             MessageBoxIcon.Question);

                    if (dlgResult == DialogResult.Yes)
                    {
                        PopulateGrid();
                        WriteExcelOutput();
                    }
                    else if (dlgResult == DialogResult.No)
                    {
                        this.Focus();
                    }
                }
                #endregion

                #region Heading Only Export Excel

                else if (dataGridView1.Rows.Count < 1)
                {
                    string msgBoxTitle = "Only Heading Export !";
                    string msgTxt = "Only Heading Row will be Exported !";

                    DialogResult dlgResult = MessageBox.Show(msgTxt, msgBoxTitle, MessageBoxButtons.OK,
                                                             MessageBoxIcon.Exclamation);
                    if (dlgResult == DialogResult.OK)
                    {
                        WriteExcelOutput();
                    }
                    else if (dlgResult == DialogResult.Cancel)
                    {
                        this.Dispose();
                    }
                }
                #endregion

                #region Final Full Export Excel

                else
                {
                    WriteExcelOutput();
                }

                #endregion
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ExportExcel", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ExportExcel", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ExportExcel", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "ExportExcel", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ExportExcel", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ExportExcel", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ExportExcel", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ExportExcel", exMessage);
            }

            #endregion
        }

        private void PopulateGrid()
        {
            #region try

            try
            {
                if (cmbImport.Text == "Product")
                {
                    #region Action cmbImport.Text == "Product"

                    ProductHeader();

                    #endregion
                }

                else if (cmbImport.Text == "Product Group")
                {
                    #region Action cmbImport.Text == "Product Group"

                    ProductCategoryHeader();

                    #endregion
                }
                else if (cmbImport.Text == "Product Type") // moded here underneth
                {
                    ProductTypeHeader();
                }
                else if (cmbImport.Text == "Customer")
                {
                    CustomerHeader();
                }
                else if (cmbImport.Text == "Customer Group")
                {
                    CustomerGroupHeader();
                }
                else if (cmbImport.Text == "Vendor")
                {
                    VendorHeader();
                }
                else if (cmbImport.Text == "Vendor Group")
                {
                    VendorGroupHeader();
                }
                else if (cmbImport.Text == "Vehicle")
                {
                    VehicleHeader();
                }
                else if (cmbImport.Text == "Bank")
                {
                    BankHeader();
                }
                else if (cmbImport.Text == "Head Name")
                {
                    OverHeadHeader();
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "PopulateGrid", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "PopulateGrid", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "PopulateGrid", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "PopulateGrid", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PopulateGrid", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PopulateGrid", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PopulateGrid", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "PopulateGrid", exMessage);
            }

            #endregion
        }

        private void WriteExcelOutput()
        {
            #region try

            try
            {
                // creating Excel Application
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

                // creating new WorkBook within Excel application
                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(System.Type.Missing);

                // creating new Excelsheet in workbook
                Microsoft.Office.Interop.Excel._Worksheet worksheet = new Microsoft.Office.Interop.Excel.Worksheet();

                // see the excel sheet behind the program
                app.Visible = false;

                // get the reference of first sheet. By default its name is Sheet1.
                // store its reference to worksheet

                worksheet = workbook.Sheets["Sheet1"] as _Worksheet;
                worksheet = workbook.ActiveSheet as _Worksheet;


                // changing the name of active sheet
                worksheet.Name = cmbImport.Text;
                //"Exported from gridview";
                // storing header part in Excel
                for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
                }

                // storing Each row and column value to excel sheet
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    }
                }
                string xportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string xportFileName = string.Format(@"{0}\{1}-output.xlsx", xportPath, cmbImport.Text);

                // save the application
                workbook.SaveAs(xportFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing,
                                Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing);

                // Exit from the application
                app.Quit();
                releaseObject(worksheet);
                releaseObject(workbook);
                releaseObject(app);
                MessageBox.Show(string.Format("Excel file created , you can find the file {0}", xportFileName));
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "WriteExcelOutput", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "WriteExcelOutput", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "WriteExcelOutput", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "WriteExcelOutput", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "WriteExcelOutput", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "WriteExcelOutput", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "WriteExcelOutput", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "WriteExcelOutput", exMessage);
            }

            #endregion
        }

        private void BrowsFile()
        {
            #region try

            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";


                //fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*";
                //BugsBD
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*|CSV files (*.csv*)|*.csv*";


                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    Program.ImportFileName = fdlg.FileName;
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
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

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "BrowsFile", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                #region NullCheck
                if (dataGridView1.RowCount <= 0)
                {
                    MessageBox.Show("There are no data to save!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (empty == true)
                {
                    MessageBox.Show("empty field can't save in database, please check ");
                    return;
                }
                if (NotNumeric == true)
                {
                    MessageBox.Show("Numeric field need numeric data only, please check ");
                    return;
                }
                if (NotDate == true)
                {
                    MessageBox.Show("Date field need date data only, please check ");
                    return;
                }
                if (NotActiveCharacter == true)
                {
                    MessageBox.Show("Is Active field only Y/N value, please check ");
                    return;
                }
                #endregion NullCheck

                if (cmbImport.Text == "UserInformation")
                {
                    UserInformationSave();
                }
              
                //else if (cmbImport.Text == "Product Opening")
                //{
                //    ProductOpeningSave();
                //}
                  


            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
               

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }

            #endregion
        }

        private void bgwSave_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void cmbImport_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void FormImport_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

                if (ChangeData == true)
                {
                    if (DialogResult.Yes != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + " Want to close without saving?",
                        this.Text,

                        MessageBoxButtons.YesNo,

                        MessageBoxIcon.Question,

                        MessageBoxDefaultButton.Button2))
                    {
                        e.Cancel = true;
                    }

                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            #endregion
        }

        private void btnCost_Click(object sender, EventArgs e)
        {
            string fileName = "";
            if (chkSame.Checked == false)
            {
                BrowsFile();
                fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return;
                }
            }
            CostingImport();
        }

        //private string[] ImportFromExcel()
        //{
        //    #region Close1
        //    string[] sqlResults = new string[4];
        //    sqlResults[0] = "Fail";
        //    sqlResults[1] = "Fail";
        //    sqlResults[2] = "";
        //    sqlResults[3] = "";
        //    #endregion Close1

        //    #region try
        //    OleDbConnection theConnection = null;
        //    TransactionTypes();

        //    try
        //    {
        //        #region Load Excel

        //        string str1 = "";
        //        string fileName = Program.ImportFileName;
        //        if (string.IsNullOrEmpty(fileName))
        //        {
        //            MessageBox.Show("Please select the right file for import");
        //            return sqlResults;
        //        }
        //        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
        //                                  + "Data Source=" + fileName + ";"
        //                                  + "Extended Properties=" + "\""
        //                                  + "Excel 12.0;HDR=YES;" + "\"";
        //        theConnection = new OleDbConnection(connectionString);
        //        theConnection.Open();
        //        OleDbDataAdapter ReceiveM = new OleDbDataAdapter("SELECT * FROM [ReceiveM$]", theConnection);
        //        System.Data.DataTable dtReceiveM = new System.Data.DataTable();
        //        ReceiveM.Fill(dtReceiveM);

        //        OleDbDataAdapter adapterReceiveD = new OleDbDataAdapter("SELECT * FROM [ReceiveD$]", theConnection);
        //        System.Data.DataTable dtReceiveD = new System.Data.DataTable();
        //        adapterReceiveD.Fill(dtReceiveD);

        //        theConnection.Close();

        //        #endregion Load Excel
        //        dtReceiveM.Columns.Add("Transection_Type");
        //        dtReceiveM.Columns.Add("Created_By");
        //        dtReceiveM.Columns.Add("LastModified_By");
        //        dtReceiveM.Columns.Add("From_BOM");
        //        dtReceiveM.Columns.Add("Total_VAT_Amount");
        //        dtReceiveM.Columns.Add("Total_Amount");

        //        foreach (DataRow row in dtReceiveM.Rows)
        //        {
        //            row["Transection_Type"] = transactionType;
        //            row["Created_By"] = Program.CurrentUser;
        //            row["LastModified_By"] = Program.CurrentUser;
        //            row["From_BOM"] = Convert.ToBoolean(IssueFromBOM == true) ? "Y" : "N";
        //            row["Total_VAT_Amount"] = "0";
        //            row["Total_Amount"] = txtTotalAmount.Text.Trim();

        //        }
        //        SAVE_DOWORK_SUCCESS = false;
        //        //sqlResults = new string[4];

        //        ReceiveDAL receiveDal = new ReceiveDAL();
        //        sqlResults = receiveDal.ImportData(dtReceiveM, dtReceiveD);
        //        SAVE_DOWORK_SUCCESS = true;
        //    }


        //    #endregion try
        //    #region catch
        //    catch (ArgumentNullException ex)
        //    {
        //        string err = ex.Message.ToString();
        //        string[] error = err.Split(FieldDelimeter.ToCharArray());
        //        FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
        //        //MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (IndexOutOfRangeException ex)
        //    {
        //        FileLogger.Log(this.Name, "ReceiveImportFromExcel", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        FileLogger.Log(this.Name, "ReceiveImportFromExcel", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (FormatException ex)
        //    {

        //        FileLogger.Log(this.Name, "ReceiveImportFromExcel", ex.Message + Environment.NewLine + ex.StackTrace);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }

        //    catch (SoapHeaderException ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }

        //        FileLogger.Log(this.Name, "ReceiveImportFromExcel", exMessage);
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    }
        //    catch (SoapException ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ReceiveImportFromExcel", exMessage);
        //    }
        //    catch (EndpointNotFoundException ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ReceiveImportFromExcel", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch (WebException ex)
        //    {
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ReceiveImportFromExcel", ex.Message + Environment.NewLine + ex.StackTrace);
        //    }
        //    catch (Exception ex)
        //    {
        //        string exMessage = ex.Message + Environment.NewLine +
        //                        ex.StackTrace;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        FileLogger.Log(this.Name, "ReceiveImportFromExcel", exMessage);
        //    }
        //    return sqlResults;

        //    #endregion
        //}

        private void CostingSave()
        {
            #region try

            try
            {
                costings.Clear();
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    decimal Quantity = 0;
                     decimal   UnitCost=0;
                     decimal AV = 0; decimal CD = 0; decimal RD = 0; decimal TVB = 0; decimal SD = 0; decimal VAT = 0; decimal TVA = 0; decimal ATV = 0;
                    decimal Other= 0;
                    decimal CostPrice = 0;

                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["Quantity"].Value.ToString()))
                    {
                        Quantity = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Quantity"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["Unit_Cost"].Value.ToString()))
                    {
                        UnitCost = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Unit_Cost"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["AV"].Value.ToString()))
                    {
                        AV = Convert.ToDecimal(dataGridView1.Rows[i].Cells["AV"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["CD"].Value.ToString()))
                    {
                        CD = Convert.ToDecimal(dataGridView1.Rows[i].Cells["CD"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["RD"].Value.ToString()))
                    {
                        RD = Convert.ToDecimal(dataGridView1.Rows[i].Cells["RD"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["TVB"].Value.ToString()))
                    {
                        TVB = Convert.ToDecimal(dataGridView1.Rows[i].Cells["TVB"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["SD"].Value.ToString()))
                    {
                        SD = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SD"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["VAT"].Value.ToString()))
                    {
                        VAT = Convert.ToDecimal(dataGridView1.Rows[i].Cells["VAT"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["TVA"].Value.ToString()))
                    {
                        TVA = Convert.ToDecimal(dataGridView1.Rows[i].Cells["TVA"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["ATV"].Value.ToString()))
                    {
                        ATV = Convert.ToDecimal(dataGridView1.Rows[i].Cells["ATV"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["Other"].Value.ToString()))
                    {
                        Other = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Other"].Value);
                    }
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells["Total"].Value.ToString()))
                    {
                        CostPrice = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Total"].Value);
                    }

                    CostingVM costing=new CostingVM();

                    costing.ProductCode = Program.RemoveStringExpresion(dataGridView1.Rows[i].Cells["Product_Code"].Value.ToString());
                    costing.ProductName = Program.RemoveStringExpresion(dataGridView1.Rows[i].Cells["Product_Name"].Value.ToString());
                    costing.BENumber = dataGridView1.Rows[i].Cells["BENumber"].Value.ToString();
                    costing.RefNo = dataGridView1.Rows[i].Cells["RefNo"].Value.ToString();
                    costing.InputDate = Convert.ToDateTime(dataGridView1.Rows[i].Cells["Input_Date"].Value).ToString("yyyy-MMM-dd HH:mm:ss");
                    costing.Quantity = Quantity;
                    costing.UnitCost = UnitCost;
                    costing.AV = AV;
                    costing.CD = CD;
                    costing.RD = RD;
                    costing.TVB = TVB;
                    costing.SDAmount = SD;
                    costing.VATAmount = VAT;
                    costing.TVA = TVA;
                    costing.ATV = ATV;
                    costing.Other = Other;
                    costing.CostPrice = CostPrice;
                    costing.CreatedBy = Program.CurrentUser;
                    costing.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                   
                    costings.Add(costing);

                }

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[2];

                //ImportDAL importDal = new ImportDAL();
                IImport importDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);

                sqlResults = importDal.ImportCosting(costings,connVM);

                SAVE_DOWORK_SUCCESS = true;

                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("CostingImport",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "VehicleImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VehicleImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "VehicleImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "VehicleImport", exMessage);
            }

            #endregion
            finally
            {
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void CostingHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 18;
            dataGridView1.Columns[0].Name = "Sl";
            dataGridView1.Columns[1].Name = "Product_Code";
            dataGridView1.Columns[2].Name = "Product_Name";
            dataGridView1.Columns[3].Name = "BENumber";
            dataGridView1.Columns[4].Name = "RefNo";
            dataGridView1.Columns[5].Name = "Input_Date";
            dataGridView1.Columns[6].Name = "Quantity";
            dataGridView1.Columns[7].Name = "Unit_Cost";
            dataGridView1.Columns[8].Name = "AV";
            dataGridView1.Columns[9].Name = "CD";//s
            dataGridView1.Columns[10].Name = "RD";
            dataGridView1.Columns[11].Name = "TVB";
            dataGridView1.Columns[12].Name = "SD";
            dataGridView1.Columns[13].Name = "VAT";
            dataGridView1.Columns[14].Name = "TVA";
            dataGridView1.Columns[15].Name = "ATV";
            dataGridView1.Columns[16].Name = "Other";
            dataGridView1.Columns[17].Name = "Total";
            
            
        }

        private void CostingImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                          "Excel 12.0;HDR=YES;" + "\"";
                OleDbConnection theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [Costing$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);

               for (int i = 0; i < dt.Rows.Count; i++)
                {
                    decimal Quantity = 0;
                    decimal UnitCost = 0;
                    decimal AV = 0;
                    decimal CD = 0;
                    decimal RD = 0;
                    decimal TVB = 0;
                    decimal SD = 0;
                    decimal VAT = 0;
                    decimal TVA = 0;
                    decimal ATV = 0;
                    decimal Other = 0;
                    decimal CostPrice = 0;

                    if (!string.IsNullOrEmpty(dt.Rows[i]["Quantity"].ToString()))
                    {
                        Quantity = Convert.ToDecimal(dt.Rows[i]["Quantity"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["Unit_Cost"].ToString()))
                    {
                        UnitCost = Convert.ToDecimal(dt.Rows[i]["Unit_Cost"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["AV"].ToString()))
                    {
                        AV = Convert.ToDecimal(dt.Rows[i]["AV"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["CD"].ToString()))
                    {
                        CD = Convert.ToDecimal(dt.Rows[i]["CD"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["RD"].ToString()))
                    {
                        RD = Convert.ToDecimal(dt.Rows[i]["RD"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["TVB"].ToString()))
                    {
                        TVB = Convert.ToDecimal(dt.Rows[i]["TVB"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["SD"].ToString()))
                    {
                        SD = Convert.ToDecimal(dt.Rows[i]["SD"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["VAT"].ToString()))
                    {
                        VAT = Convert.ToDecimal(dt.Rows[i]["VAT"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["TVA"].ToString()))
                    {
                        TVA = Convert.ToDecimal(dt.Rows[i]["TVA"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["ATV"].ToString()))
                    {
                        ATV = Convert.ToDecimal(dt.Rows[i]["ATV"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["Other"].ToString()))
                    {
                        Other = Convert.ToDecimal(dt.Rows[i]["Other"]);
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["Total"].ToString()))
                    {
                        CostPrice = Convert.ToDecimal(dt.Rows[i]["Total"]);
                    }
              
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);
                    dataGridView1["Sl", dataGridView1.RowCount - 1].Value = (i + 1).ToString();
                    dataGridView1["Product_Code", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Product_Code"].ToString();
                    dataGridView1["Product_Name", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Product_Name"].ToString();
                    dataGridView1["BENumber", dataGridView1.RowCount - 1].Value = dt.Rows[i]["BENumber"].ToString();
                    dataGridView1["RefNo", dataGridView1.RowCount - 1].Value = dt.Rows[i]["RefNo"].ToString();
                    #region Checking Date is null or different formate
                    CommonImportDAL cImport = new CommonImportDAL();
                    bool IsInputDate;
                    IsInputDate = cImport.CheckDate(dt.Rows[i]["Input_Date"].ToString().Trim());
                    if (IsInputDate != true)
                    {
                        MessageBox.Show("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                        return;
                    }
                    #endregion Checking Date is null or different formate

                    dataGridView1["Input_Date", dataGridView1.RowCount - 1].Value = Convert.ToDateTime(dt.Rows[i]["Input_Date"]).ToString("yyyy-MMM-dd HH:mm:ss");
                    dataGridView1["Quantity", dataGridView1.RowCount - 1].Value = Quantity;
                    dataGridView1["Unit_Cost", dataGridView1.RowCount - 1].Value = UnitCost;
                    dataGridView1["AV", dataGridView1.RowCount - 1].Value = AV;
                    dataGridView1["CD", dataGridView1.RowCount - 1].Value = CD;
                    dataGridView1["RD", dataGridView1.RowCount - 1].Value = RD;
                    dataGridView1["TVB", dataGridView1.RowCount - 1].Value = TVB;
                    dataGridView1["SD", dataGridView1.RowCount - 1].Value = SD;
                    dataGridView1["VAT", dataGridView1.RowCount - 1].Value = VAT;
                    dataGridView1["TVA", dataGridView1.RowCount - 1].Value = TVA;
                    dataGridView1["ATV", dataGridView1.RowCount - 1].Value = ATV;
                    dataGridView1["Other", dataGridView1.RowCount - 1].Value = Other;
                    dataGridView1["Total", dataGridView1.RowCount - 1].Value = CostPrice;

                }

               theConnection.Close();

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CostingData", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CostingData", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CostingData", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CostingData", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CostingData", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CostingData", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "CostingData", exMessage);
            }

            #endregion
        }

        private void UOMImport()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                //string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                //                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                //                          "Excel 12.0;HDR=YES;" + "\"";
                //OleDbConnection theConnection = new OleDbConnection(connectionString);
                //theConnection.Open();
                //OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [UOM$]", theConnection);
                DataSet theDS = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                ImportVM paramVm=new ImportVM();
                paramVm.FilePath=fileName;
                paramVm.TableName="UOM";

                //IImport importDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);
                //dt = importDal.GetDataTableFromExcel(paramVm);
                dt = new ImportDAL().GetDataTableFromExcel(paramVm);

                //theDataAdapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dataGridView1.Rows.Add(NewRow);

                    dataGridView1["UOM Id", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Id"].ToString();
                    dataGridView1["UOM Name", dataGridView1.RowCount - 1].Value = dt.Rows[i]["UOM Name"].ToString();
                    dataGridView1["UOM Code", dataGridView1.RowCount - 1].Value = dt.Rows[i]["UOM Code"].ToString();
                    dataGridView1["Comments", dataGridView1.RowCount - 1].Value = dt.Rows[i]["Comments"].ToString();
                    dataGridView1["Active Status", dataGridView1.RowCount - 1].Value = dt.Rows[i]["ActiveStatus"].ToString();
                }
                theConnection.Close();

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "UOMImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UOMImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UOMImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "UOMImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "UOMImport", exMessage);
            }

            #endregion
        }

        private void UOMHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Name = "UOM Id";
            dataGridView1.Columns[1].Name = "UOM Name";
            dataGridView1.Columns[2].Name = "UOM Code";
            dataGridView1.Columns[3].Name = "Comments";
            dataGridView1.Columns[4].Name = "Active Status";
        }

        private void UOMSave()
        {
            try
            {
                uoms.Clear();
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    UOMNameVM uom = new UOMNameVM();
                    uom.UOMID = dataGridView1.Rows[i].Cells["UOM Id"].Value.ToString();
                    uom.UOMName = Program.RemoveStringExpresion(dataGridView1.Rows[i].Cells["UOM Name"].Value.ToString());
                    uom.UOMCode = Program.RemoveStringExpresion(dataGridView1.Rows[i].Cells["UOM Code"].Value.ToString());
                    uom.Comments = dataGridView1.Rows[i].Cells["Comments"].Value.ToString();
                    uom.ActiveStatus = dataGridView1.Rows[i].Cells["Active Status"].Value.ToString();
                    uom.CreatedBy = Program.CurrentUser;
                    uom.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    uoms.Add(uom);

                }

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[2];

                //ImportDAL importDal = new ImportDAL();
                IImport importDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);

                sqlResults = importDal.ImportUOM_Names(uoms,connVM);

                SAVE_DOWORK_SUCCESS = true;

                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("ImportUOM", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length>1)
                {
                    MessageBox.Show(error[error.Length-1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "UOMSave", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "UOMSave", exMessage);
            }
            #endregion
            finally
            {
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void TrackingImport(System.Data.DataTable dtProduct, System.Data.DataTable dtTrack)
        {
            try
            {


                DataRow[] TrackingRaws;
                trackings.Clear();
                trackings = new List<TrackingVM>();
                for (int item = 0; item < dtProduct.Rows.Count; item++)
                {
                    string Id = dtProduct.Rows[item]["Sl"].ToString();
                    string ItemCode = dtProduct.Rows[item]["Code"].ToString();
                    string ItemName = dtProduct.Rows[item]["ProductName"].ToString();
                    string OpeningQty = dtProduct.Rows[item]["OpeningQuantity"].ToString();
                    string TotalPrice = dtProduct.Rows[item]["TotalPrice"].ToString();

                    TrackingRaws = dtTrack.Select("ID='" + Id + "'");


                    if (TrackingRaws != null && TrackingRaws.Length > 0)
                    {
                        int lineNoT = 1;

                        foreach (DataRow row in TrackingRaws)
                        {
                            string itemCode = row["Item_Code"].ToString().Trim();
                            string itemName = row["Item_Name"].ToString().Trim();
                            string heading1 = row["Heading_1"].ToString().Trim();
                            string heading2 = row["Heading_2"].ToString().Trim();
                            string quantity = "0";
                            string totalPrice = "0";

                            TrackingVM trackingVm = new TrackingVM();
                            trackingVm.TrackingLineNo = lineNoT.ToString();
                            trackingVm.ProductCode = itemCode;
                            trackingVm.ProductName = itemName;
                            trackingVm.Heading1 = heading1;
                            trackingVm.Heading2 = heading2;
                            trackingVm.Quantity = 1;
                            decimal unitPrice = Convert.ToDecimal(Convert.ToDecimal(TotalPrice.ToString()) / Convert.ToDecimal(OpeningQty.ToString()));
                            trackingVm.UnitPrice = unitPrice;

                            trackingVm.IsPurchase = "N";
                            trackingVm.IsIssue = "N";
                            trackingVm.IsReceive = "N";
                            trackingVm.IsSale = "N";

                            trackings.Add(trackingVm);
                            lineNoT++;
                        }
                    }
                }
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }


            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "UOMSave", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "UOMSave", exMessage);
            }
            #endregion
        }

        private void chkTrack_CheckedChanged(object sender, EventArgs e)
        {
            try
            {


                if (chkTrack.Checked == true)
                {
                    dgvSerialTrack.Top = dataGridView1.Top;
                    dgvSerialTrack.Left = dataGridView1.Left;
                    dgvSerialTrack.Height = dataGridView1.Height;
                    dgvSerialTrack.Width = dataGridView1.Width;

                    dataGridView1.Visible = false;
                    dgvSerialTrack.Visible = true;

                    dgvSerialTrack.Rows.Clear();
                    for (int i = 0; i < trackings.Count; i++)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();
                        dgvSerialTrack.Rows.Add(NewRow);

                        dgvSerialTrack["LineNoS", dgvSerialTrack.RowCount - 1].Value = trackings[i].TrackingLineNo;
                        dgvSerialTrack["ProductCodeS", dgvSerialTrack.RowCount - 1].Value = trackings[i].ProductCode;
                        dgvSerialTrack["ProductNameS", dgvSerialTrack.RowCount - 1].Value = trackings[i].ProductName;
                        dgvSerialTrack["ItemNoS", dgvSerialTrack.RowCount - 1].Value = trackings[i].ItemNo;
                        dgvSerialTrack["Heading1", dgvSerialTrack.RowCount - 1].Value = trackings[i].Heading1;
                        dgvSerialTrack["Heading2", dgvSerialTrack.RowCount - 1].Value = trackings[i].Heading2;
                        dgvSerialTrack["QuantityS", dgvSerialTrack.RowCount - 1].Value = "1";
                        dgvSerialTrack["Value", dgvSerialTrack.RowCount - 1].Value = trackings[i].UnitPrice;
                    }
                }
                else
                {
                    dataGridView1.Visible = true;
                    dgvSerialTrack.Visible = false;
                }
            }

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }


            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "chkTrack_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkTrack_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkTrack_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "chkTrack_CheckedChanged", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkTrack_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkTrack_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkTrack_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "chkTrack_CheckedChanged", exMessage);
            }
            #endregion
        }

        private void BranchProfileSave()
        {

            try
            {

                BranchProfile.Clear();
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    BranchProfileVM branchProfile = new BranchProfileVM();

                    branchProfile.BranchCode = Program.RemoveStringExpresion(dataGridView1.Rows[i].Cells["BranchCode"].Value.ToString());
                    branchProfile.BranchName = Program.RemoveStringExpresion(dataGridView1.Rows[i].Cells["BranchName"].Value.ToString());
                    branchProfile.BranchLegalName = Program.RemoveStringExpresion(dataGridView1.Rows[i].Cells["BranchLegalName"].Value.ToString());
                    branchProfile.Address = dataGridView1.Rows[i].Cells["Address"].Value.ToString();
                    branchProfile.City = dataGridView1.Rows[i].Cells["City"].Value.ToString();
                    branchProfile.ZipCode = dataGridView1.Rows[i].Cells["ZipCode"].Value.ToString();
                    branchProfile.TelephoneNo = dataGridView1.Rows[i].Cells["TelephoneNo"].Value.ToString();
                    branchProfile.FaxNo = dataGridView1.Rows[i].Cells["FaxNo"].Value.ToString();
                    branchProfile.Email = dataGridView1.Rows[i].Cells["Email"].Value.ToString();
                    branchProfile.ContactPerson = dataGridView1.Rows[i].Cells["ContactPerson"].Value.ToString();
                    branchProfile.ContactPersonDesignation = dataGridView1.Rows[i].Cells["ContactPersonDesignation"].Value.ToString();
                    branchProfile.ContactPersonTelephone = dataGridView1.Rows[i].Cells["ContactPersonTelephone"].Value.ToString();
                    branchProfile.ContactPersonEmail = dataGridView1.Rows[i].Cells["ContactPersonEmail"].Value.ToString();
                    branchProfile.VatRegistrationNo = dataGridView1.Rows[i].Cells["VatRegistrationNo"].Value.ToString();
                    branchProfile.BIN = dataGridView1.Rows[i].Cells["BIN"].Value.ToString();
                    branchProfile.TINNo = dataGridView1.Rows[i].Cells["TINNo"].Value.ToString();
                    branchProfile.Comments = dataGridView1.Rows[i].Cells["Comments"].Value.ToString();
                    branchProfile.ActiveStatus = dataGridView1.Rows[i].Cells["ActiveStatus"].Value.ToString();
                    branchProfile.IsCentral = dataGridView1.Rows[i].Cells["IsCentral"].Value.ToString()=="Y"?true:false;
                    branchProfile.CreatedBy = Program.CurrentUser;
                    branchProfile.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    branchProfile.LastModifiedBy = Program.CurrentUser;
                    branchProfile.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    BranchProfile.Add(branchProfile);
                }
                Productresult = string.Empty;

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;
                //bgwBranchProfile.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "BranchProfileSave", exMessage);
            }
            #endregion

        }

        private void bgwBranchProfile_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region statement

                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[2];


                ImportDAL importDal = new ImportDAL();
                //IImport importDal = OrdinaryVATDesktop.GetObject<ImportDAL, ImportRepo, IImport>(OrdinaryVATDesktop.IsWCF);

                sqlResults = importDal.ImportBranchProfile(BranchProfile,BranchMapDetails, connVM);

                //SaleDAL saleDal = new SaleDAL();
                //sqlResults = saleDal.SalesInsert(saleMaster, saleDetails, saleExport);

                SAVE_DOWORK_SUCCESS = true;

                #endregion
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
                FileLogger.Log(this.Name, "bgwBranchProfile_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwBranchProfile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                #region Start

                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwBranchProfile_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }


                    }

                #endregion

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
                FileLogger.Log(this.Name, "bgwBranchProfile_RunWorkerCompleted", exMessage);
            }
            #endregion
            #region Final
            finally
            {
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }
            #endregion Final

        }

        

    }
}