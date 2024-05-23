using System;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;

using System.IO;
using CrystalDecisions.CrystalReports.Engine;

//
using SymphonySofttech.Reports.Report;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.License;
using OfficeOpenXml;
using VATServer.Ordinary;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using SymphonySofttech.Reports;
using VATServer.Interface;
using VATDesktop.Repo;


namespace VATClient.ReportPreview
{
    public partial class FormRptStock : Form
    {
        public FormRptStock()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }
        StockMovementVM vm = new StockMovementVM();

        SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        string POSTCMB = string.Empty;

        private string vdtpStartDate;
        private string vdtpToDate;
        private bool ExcelRpt = false;
        private bool ExcelRptDetail = false;
        private string itemNo = "";
        private string vat6_2_1 = "";
        private string pType = "";
        private string CategoryId = "";
        private string gName = "";
        private string UOM = "";
        private string Uomconversion = "1";
        private string ReportType = "Details";
        public int SenderBranchId = 0;
        public int BranchId;
        private decimal UOMConversion = 1;

        private string UOMFromParam = "";
        private string UOMToParam = "";
        public string FormNumeric = string.Empty;
        public string CompanyCode = string.Empty;
        //string FromDate, ToDate;
        //public string VFIN = "404";

        #region Global Variables For BackGroundWorker
        private ReportDocument reportDocument = new ReportDocument();

        private DataSet ReportResult;
        private DataTable uomResult;

        private string cmbTradingText,
                       cmbNonStockText,
                       cmbPostText1, cmbPostText2;


        #endregion

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            //No DAL Method
            try
            {
                Program.fromOpen = "Me";
                Program.R_F = "";

                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//
                    txtUOM.Text = selectedRow.Cells["UOM"].Value.ToString();



                }
                UOMFromParam = txtUOM.Text.Trim();
                UOMToParam = string.Empty;
                UomLoad();

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchProduct_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchProduct_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProduct_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchProduct_Click", exMessage);
            }
            #endregion Catch
        }
        private void UomLoad()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtItemNo.Text.Trim()))
                {
                    cmbUOM.DataSource = null;
                    cmbUOM.Items.Clear();

                    IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                    string[] cvals = new string[] { UOMFromParam, UOMToParam, "Y" };
                    string[] cfields = new string[] { "UOMFrom", "UOMTo", "ActiveStatus like" };
                    uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);
                    cmbUOM.DataSource = uomResult;
                    cmbUOM.DisplayMember = "UOMTo";
                    cmbUOM.ValueMember = "UOMTo";
                    cmbUOM.Text = txtUOM.Text;
                    UOMToParam = cmbUOM.Text;

                    FindUOMConversuon();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void FindUOMConversuon()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtUOM.Text.Trim()))
                {


                    IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                    string[] cvals = new string[] { UOMFromParam, UOMToParam, "Y" };
                    string[] cfields = new string[] { "UOMFrom", "UOMTo", "ActiveStatus like" };
                    uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);
                    if (uomResult != null && uomResult.Rows.Count > 0)
                    {
                        txtConvertion.Text = Program.ParseDecimalObject(uomResult.Rows[0]["Convertion"].ToString());
                        decimal UomConversion = Convert.ToDecimal(txtConvertion.Text.Trim());
                        UOMConversion = UomConversion;
                        Uomconversion = Convert.ToString(UOMConversion);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void btnSearchCategory_Click(object sender, EventArgs e)
        {

            try
            {
                Program.fromOpen = "Me";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result != "")
                {
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtPGroupId.Text = ProductCategoryInfo[0];
                    txtPGroup.Text = ProductCategoryInfo[1];
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchCategory_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchCategory_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchCategory_Click", exMessage);
            }
            #endregion Catch

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                txtProductName.Text = "";
                txtPGroup.Text = "";
                txtProductType.Text = "";
                txtItemNo.Text = "";
                txtPGroupId.Text = "";
                cmbPost.SelectedIndex = -1;
                dtpFromDate.Checked = false;
                dtpToDate.Checked = false;
                chkCategoryLike.Checked = false;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            #endregion Catch
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss fff tt"));
            this.Close();
        }

        private void FormRptStock_Load(object sender, EventArgs e)
        {
            cmbPost.Text = "Y";
            PTypeSearch();
            #region Branch DropDown

            string[] Conditions = new string[] { "ActiveStatus='Y'" };
            cmbBranchName = new CommonDAL().ComboBoxLoad(cmbBranchName, "BranchProfiles", "BranchID", "BranchName", Conditions, "varchar", true, true, connVM);

            if (rbtnWeastage.Checked)
            {
                //panel2.Visible = false;
                chkSummery.Visible = false;
                cmbReportType.Visible = false;
                label2.Visible = false;

            }

            //else
            //{
            //    cmbPost.Text="Y";
            //    cmbPost.Enabled = false;
            //}

            if (SenderBranchId > 0)
            {
                cmbBranchName.SelectedValue = SenderBranchId;
            }
            else
            {
                cmbBranchName.SelectedValue = Program.BranchId;

            }

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
            cmbBranchName.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

            #endregion

            #region Preview Only

            CommonDAL commonDal = new CommonDAL();
            FormNumeric = commonDal.settingsDesktop("DecimalPlace", "FormNumeric", null, connVM);
            cmbDecimal.Text = FormNumeric;

            string PreviewOnly = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM);

             CompanyCode = commonDal.settings("CompanyCode", "Code");

            if (PreviewOnly.ToLower() == "n")
            {
                cmbPost.Text = "Y";
                cmbPost.Visible = false;
                label9.Visible = false;
            }

            #endregion

        }
        private void PTypeSearch()
        {
            cmbReportType.Text = "Report_1";
            cmbProductType.Items.Clear();

            ProductDAL productTypeDal = new ProductDAL();
            cmbProductType.DataSource = productTypeDal.ProductTypeList;
            cmbProductType.SelectedIndex = -1;

        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            ExcelRpt = false;
            ExcelRptDetail = false;
            #region Report Preview

            btnPreviewLoad();

            #endregion
        }

        private void btnPreviewLoad()
        {

            #region Check Point

            if (Program.CheckLicence(dtpToDate.Value) == true)
            {
                MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                return;
            }

            NullCheck();


            #endregion

            #region Button Stats

            this.progressBar1.Visible = true;
            this.btnPreview.Enabled = false;

            #endregion

            #region Report Date

            MISReport _report = new MISReport();
            _report.dtpFromDate = dtpFromDate.Checked;
            _report.dtpToDate = dtpToDate.Checked;


            #endregion

            #region Value Assign

            Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
            FormNumeric = cmbDecimal.Text.Trim();
            UOM = cmbUOM.Text;
            BranchId = Convert.ToInt32(cmbBranchName.SelectedValue);

            OrdinaryVATDesktop.FontSize = Program.FontSize;

            #endregion

            #region Condtional Values

            if (rbtnItem.Checked)
            {
                itemNo = txtItemNo.Text;

                pType = "";
                CategoryId = "";
            }
            else if (rbtnType.Checked)
            {
                itemNo = "";

                pType = cmbProductType.Text;
                CategoryId = "";
            }
            else if (rbtnCategory.Checked)
            {
                itemNo = "";
                pType = "";
                gName = "";

                CategoryId = txtPGroupId.Text;
                gName = txtPGroup.Text;
            }
            vat6_2_1 = "N";
            if (chkTrading.Checked)
            {
                vat6_2_1 = "Y";
            }
            #endregion

            #region Report Control
            vm = new StockMovementVM();
            vm.ItemNo = itemNo;
            vm.StartDate = vdtpStartDate;
            vm.ToDate = vdtpToDate;
            vm.BranchId = BranchId;
            vm.Post1 = cmbPostText1;
            vm.Post2 = cmbPostText2;
            vm.ProductType = pType;
            vm.CategoryId = CategoryId;
            vm.ProductGroupName = gName;
            vm.CategoryLike = chkCategoryLike.Checked;
            vm.FormNumeric = FormNumeric;
            vm.CurrentUserID = Program.CurrentUserID;
            vm.Branchwise = BranchId != 0;
            vm.chkTrading = chkTrading.Checked ? "Y" : "N";


            FileLogger.Log(this.Name, "btnPreviewLoad", JsonConvert.SerializeObject(vm));

            if (rbtnWeastage.Checked)
            {
                WastageReport();
            }
            else
            {
                backgroundWorkerPreview.RunWorkerAsync();
            }

            #endregion

        }

        #region Wastage Report

        private void WastageReport()
        {

            try
            {

                MISReport _report = new MISReport();

                #region Report Param

                string ParamFromDate;
                string ParamToDate;
                string ProductName;
                string ProductType;

                if (dtpFromDate.Checked)
                {
                    ParamFromDate = dtpFromDate.Value.ToString("dd-MMM-yyyy").ToString();
                }
                else
                {
                    ParamFromDate = "All";
                }

                if (dtpToDate.Checked)
                {
                    ParamToDate = dtpToDate.Value.ToString("dd-MMM-yyyy").ToString();
                }
                else
                {
                    ParamToDate = "All";
                }
                if (rbtnItem.Checked)
                {
                    ProductName = txtProductName.Text;
                }
                else
                {
                    ProductName = "All";
                }
                if (rbtnType.Checked)
                {
                    ProductType = cmbProductType.Text;
                }
                else
                {
                    ProductType = "All";
                }

                #endregion

                reportDocument = _report.Wastage(itemNo, CategoryId, pType, cmbPostText1, cmbPostText2, vdtpStartDate, vdtpToDate, BranchId, ParamFromDate, ParamToDate, ProductName, ProductType, connVM);

                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(reportDocument);
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();

                #region Old


                #endregion

            }

            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "WastageReport", exMessage);
            }
            #endregion Catch
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        #endregion

        #region Background Worker Events

        private void NullCheck()
        {
            //cmbPostText1 = "Y";
            //cmbPostText2 = "N";
            try
            {
                POSTCMB = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";

                if (string.IsNullOrWhiteSpace(cmbPost.Text.Trim()))
                {
                    cmbPostText1 = "Y";
                    cmbPostText2 = "N";
                }
                else if (cmbPost.Text == "Y")
                {
                    cmbPostText1 = "Y";
                    cmbPostText2 = "Y";
                }
                else if (cmbPost.Text == "N")
                {
                    cmbPostText1 = "N";
                    cmbPostText2 = "N";
                }
                //else
                //{
                //    cmbPostText1 = cmbPost.Text.Trim();
                //    cmbPostText2 = cmbPost.Text.Trim();
                //}
                if (dtpFromDate.Checked == false)
                {
                    vdtpStartDate = "";

                    vdtpStartDate = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");//=Convert.ToDateTime(Program.vMinDate);

                }
                else
                {
                    vdtpStartDate = dtpFromDate.Value.ToString("yyyy-MMM-dd").ToString();// Convert.ToDateTime(dtpFromDate.Value.ToString("yyyy-MMM-dd"));
                }
                if (dtpToDate.Checked == false)
                {
                    vdtpToDate = "";
                    vdtpToDate = dtpToDate.MaxDate.ToString("yyyy-MMM-dd");//Convert.ToDateTime(Program.vMaxDate);
                }
                else
                {
                    vdtpToDate = dtpToDate.Value.ToString("yyyy-MMM-dd").ToString();//  Convert.ToDateTime(dtpToDate.Value.ToString("yyyy-MMM-dd"));
                }

            }
            #region Catch
           
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            #endregion Catch
        }

        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            ReportResult = new DataSet();
            ProductDAL productDal = new ProductDAL();

            vm.ExcelRptDetail = ExcelRptDetail;
            ReportResult = productDal.StockMovement(vm, null, null, connVM);

            CommonDAL commonDal = new CommonDAL();
            string companyCode = commonDal.settingValue("DayEnd", "BigDataProcess");
            //companyCode = "";

            if (string.Equals(companyCode, "Y", StringComparison.InvariantCultureIgnoreCase) && ExcelRptDetail)
            {
                e.Result = ExportToExcel();
            }
        }

        private string ExportToExcel()
        {
            try
            {
                // copy existing excel 

                // get values from table

                // append to copied excel

                // pass excel path to completed method


                string origin = Program.AppPath + @"\Templates\Stock_Report.xlsx";
                string destination = Program.AppPath + @"\Excel Files\Stock_Report.xlsx";

                File.Copy(origin, destination, true);
                FileInfo file = new FileInfo(destination);

                ProductDAL productDal = new ProductDAL();
                using (ExcelPackage excel = new ExcelPackage(file))
                {
                    ExcelWorksheet workSheet = excel.Workbook.Worksheets["Stock"];
                    int rows = 0;
                    int loopCount = productDal.GetStock_Count();
                    for (int i = 0; i < loopCount; i++) 
                    {
                        int endRow = workSheet.Dimension == null ? 1 : workSheet.Dimension.End.Row;

                        DataTable dt = productDal.GetStock_Split();

                        workSheet.Cells[endRow+1, 1].LoadFromDataTable(dt, false);

                        productDal.Stock_Update();

                        excel.Save();

                        rows += dt.Rows.Count;

                    }

                }

                return destination;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {

                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                    return;
                }

                #region Check Point

                if (ReportResult.Tables.Count <= 0 || ReportResult.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                #endregion

               
                if (ExcelRpt)
                {

                    ExcelPreview();
                }
                else if (ExcelRptDetail)
                {
                    string companyCode = new CommonDAL().settingValue("DayEnd", "BigDataProcess");
                    //CompanyCode = "";
                    if (string.Equals(companyCode, "Y", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string path = (string)e.Result;
                        ProcessStartInfo psi = new ProcessStartInfo(path)
                        {
                            UseShellExecute = true
                        };
                        Process.Start(psi);
                    }
                    else
                    {
                        ExcelPreviewDetails();
                    }

                }
                else
                {

                    //formula
                    BranchProfileDAL _branchDAL = new BranchProfileDAL();

                    string BranchName = "All";

                    if (BranchId != 0)
                    {
                        DataTable dtBranch = _branchDAL.SelectAll(BranchId.ToString(), null, null, null, null, true,connVM);
                        BranchName = "[" + dtBranch.Rows[0]["BranchCode"] + "] " + dtBranch.Rows[0]["BranchName"];
                    }
                    //end formula
                    #region Declarations

                    DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                    FormulaFieldDefinitions crFormulaF;
                    CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                    string reportPath = @"D:\VATProject\BitbucketCloud\vatdesktop\SymphonySofttech.Reports\Report\";
                    ReportDocument objrpt = new ReportDocument();

                    #endregion

                    #region Report Switch

                    if (ReportType == "Report_1")
                    {
                        if ( chkSummery.Checked )
                        {
                            objrpt = new RptSummaryStockMovement();

                        }
                        else
                        {
                            objrpt = new RptStockMovement();

                        }
                        //objrpt.Load(reportPath + @"\RptStockMovement.rpt");
                    }
                    else if (ReportType == "Report_2")
                    {
                        objrpt = new RptStockMovement2();

                    }
                    else if (ReportType == "Report_3")
                    {

                        if (chkSummery.Checked)
                        {
                            objrpt = new RptSummaryStockMovement3();

                        }
                        else
                        {
                            objrpt = new RptStockMovement3();

                        }

                        //objrpt = new RptStockMovement3();

                        //objrpt = new ReportDocument();
                        //objrpt.Load(reportPath + @"\RptStockMovement3.rpt");

                    }
                    else if (ReportType == "Report_4")
                    {
                        objrpt = new RptStockMovement4();
                        //objrpt.Load(reportPath + @"\RptStockMovement4.rpt");

                    }
                    else
                    {
                        if (chkSummery.Checked)
                        {
                            objrpt = new RptSummaryStockMovement();

                        }
                        else
                        {
                            objrpt = new RptStockMovement();

                        }
                    }

                    #endregion


                    #region Complete

                    DataTable dtStock =  ReportResult.Tables[0];
                    if (chkSummery.Checked)
                    {
                         dtStock = ReportResult.Tables[1];
                         dtStock = OrdinaryVATDesktop.DtColumnNameChange(dtStock, "ClosingValue", "ClosingAmount");

                        dtStock.TableName = "dsSummaryStockMovement";
                    }
                    #region Debug Point

                    //////DataRow[] drSort = dtStock.Select("", "ProductCode asc");
                    //////DataTable dtSort = drSort.CopyToDataTable();



                    //////DataRow[] drSummary = ReportResult.Tables[1].Select("ItemNo = '281' OR ItemNo = '336'");
                    //////DataTable dtSummary = drSummary.CopyToDataTable();



                    //////DataRow[] dr = dtStock.Select("ItemNo = '281' OR ItemNo = '336'");

                    //////DataTable dt = dr.CopyToDataTable();

                    //////dtStock = new DataTable();
                    //////dtStock = dtSort;

                    #endregion

                    dtStock = OrdinaryVATDesktop.DtDeleteColumns(dtStock, new string[] { "ItemNo" });
                    dtStock = OrdinaryVATDesktop.DtColumnNameChange(dtStock, "ProductCode", "ItemNo");
                    dtStock = dtStock.Columns.Contains("ProductType") ?
                        dtStock.Select("ProductType <> 'Overhead'").CopyToDataTable() :
                        dtStock.Select("ItemType <> 'Overhead'").CopyToDataTable();



                    #region Set Data Source

                    objrpt.SetDataSource(dtStock);

                    #endregion
                    #region Settings Load

                    CommonDAL _cDal = new CommonDAL();

                    DataTable settingsDt = new DataTable();

                    if (settingVM.SettingsDTUser == null)
                    {
                        settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                    }

                    #endregion


                    #region Formula Fields

                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UserName", Program.CurrentUser);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "CompanyName", Program.CompanyName);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address1", Program.Address1);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address2", Program.Address2);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Address3", Program.Address3);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "TelephoneNo", Program.TelephoneNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FaxNo", Program.FaxNo);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOM", UOM);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "UOMConversion", Uomconversion);

                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SummaryOpeningQty", dtStock.Rows[0]["OpeningQuantity"].ToString());
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SummaryOpeningValue", dtStock.Rows[0]["OpeningValue"].ToString());

                    //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SummaryClosingQty", dtStock.Rows[dtStock.Rows.Count-1]["ClosingQuantity"].ToString());
                    //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "SummaryClosingValue", dtStock.Rows[dtStock.Rows.Count - 1]["ClosingValue"].ToString());

                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "Stock_1", chkStock.Checked ? "Y" : "N");
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "ReportName", "Stock Information");

                    objrpt.DataDefinition.FormulaFields["Summery"].Text = chkSummery.Checked ? "'Y'" : "'N'";
                    ////objrpt.DataDefinition.FormulaFields["FormNumeric"].Text = "'" + FormNumeric + "'";
                    //objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + BranchName + "'";

                    #region Conditionals


                    if (rbtnItem.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PProduct", "[All]");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PProduct", txtProductName.Text.Trim());
                    }


                    if (rbtnCategory.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PCategory", "[All]");
                    }
                    else
                    {
                        objrpt.DataDefinition.FormulaFields["PCategory"].Text = "'" + txtPGroup.Text.Trim() + "'  ";

                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PCategory", txtPGroup.Text.Trim());

                    }

                    if (rbtnType.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PType", "[All]");

                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PType", txtProductType.Text.Trim());

                    }

                    if (dtpFromDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateFrom", "[All]");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateFrom", dtpFromDate.Value.ToString("dd/MMM/yyyy"));
                    }

                    if (dtpToDate.Checked == false)
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateTo", "[All]");
                    }
                    else
                    {
                        _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "PDateTo", dtpToDate.Value.ToString("dd/MMM/yyyy"));
                    }

                    #endregion


                    crFormulaF = objrpt.DataDefinition.FormulaFields;
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FormNumeric", FormNumeric);
                    _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "BranchName", BranchName);

                    #endregion


                    #region Show Report

                    FormReport reports = new FormReport();
                    //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(objrpt);

                    if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                    {
                        reports.WindowState = FormWindowState.Maximized;
                    }

                    reports.Show();

                    #endregion

                    #endregion
                }
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

                               exMessage);
            }

            #endregion
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    string result = FormProductTypeSearch.SelectOne();
            //    if (result == "") { return; }
            //    else//if (result == ""){return;}else//if (result != "")
            //    {
            //        string[] TypeInfo = result.Split(FieldDelimeter.ToCharArray());

            //        txtProductType.Text = TypeInfo[1];
            //    }
            //}
            //#region Catch
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log(this.Name, "button3_Click", exMessage);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button3_Click", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (Exception ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "button3_Click", exMessage);
            //}
            //#endregion Catch
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtProductType.Text = cmbProductType.Text;
        }

        private void chkCategoryLike_Click(object sender, EventArgs e)
        {
            txtPGroup.ReadOnly = true;

            if (chkCategoryLike.Checked)
            {
                txtPGroup.ReadOnly = false;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            ExcelRpt = true;
            ExcelRptDetail = false;

            btnPreviewLoad();
        }
        private void ExcelPreview()
        {
            try
            {
                DataSet Ds = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();

                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                dt1.TableName = "Copy";

                dt = new DataTable();

                if (ReportResult.Tables.Count == 1)
                {
                    dt = ReportResult.Tables[0];

                }
                else
                {
                    dt = ReportResult.Tables[1];

                }
                if (chkTrading.Checked)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Columns.Contains("UnitCost"))
                        {
                            dt.Columns.Remove("UnitCost");
                        }
                    }
                }

                dt1 = dt.Copy();
                string ParamFromDate;
                string ParamToDate;


                if (dtpFromDate.Checked)
                {
                    ParamFromDate = dtpFromDate.Value.ToString("dd-MMM-yyyy").ToString();
                }
                else
                {
                    ParamFromDate = "All";
                }

                if (dtpToDate.Checked)
                {
                    ParamToDate = dtpToDate.Value.ToString("dd-MMM-yyyy").ToString();
                }
                else
                {
                    ParamToDate = "All";
                }


                string[] ReportHeaders = new string[] {
                    Program.CompanyName
                     ,"Stock Summery","Form Date:"+ ParamFromDate+"                To Date:"+ParamToDate
                };


                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Stock");


                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    workSheet.Cells[i + 1, 1, (i + 1), (dt.Columns.Count)].Merge = true;
                    workSheet.Cells[i + 1, 1, (i + 1), (dt.Columns.Count)].Style.WrapText = true;
                    workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i]);
                }

                workSheet.Cells[ReportHeaders.Length + 2, 1].LoadFromDataTable(dt, true);


                int TableHeadRow = 0;
                TableHeadRow = ReportHeaders.Length + 2;

                int RowCount = 0;
                RowCount = dt.Rows.Count;

                int ColumnCount = 0;
                ColumnCount = dt.Columns.Count;

                int GrandTotalRow = 0;
                GrandTotalRow = TableHeadRow + RowCount + 1;

                int colNumber = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        workSheet.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {

                        workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total
                        workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, colNumber].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                        #endregion
                    }

                }


                workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                workSheet.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                string p_strPath = Program.AppPath + @"\Excel Files\" + DateTime.Now.ToString("dd_MMM_yyyy_HH_mm_ss") + "_Stock_Report.xlsx";
                if (File.Exists(p_strPath))
                    File.Delete(p_strPath);

                FileStream objFileStrm = File.Create(p_strPath);
                objFileStrm.Close();

                //Write content to excel file    
                File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
                MessageBox.Show("File Save " + p_strPath + " Successfully");
                ProcessStartInfo psi = new ProcessStartInfo(p_strPath);
                psi.UseShellExecute = true;
                Process.Start(psi);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void ExcelPreviewDetails()
        {
            try
            {
                //DataSet Ds = new DataSet();
                //ReportDSDAL reportDsdal = new ReportDSDAL();

                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                dt1.TableName = "Copy";

                dt = new DataTable();
                dt = ReportResult.Tables[0];
                if (chkTrading.Checked)
                {
                    dt.Columns.Remove("UnitCost");
                }
                string[] DeleteColumnName = { "ItemNo", "ItemNo1", "ClosingQuantityFinal", "ClosingAmountFinal" };

                dt = OrdinaryVATDesktop.DtDeleteColumns(dt, DeleteColumnName);

                //dt1 = dt.Copy();
                string ParamFromDate;
                string ParamToDate;


                if (dtpFromDate.Checked)
                {
                    ParamFromDate = dtpFromDate.Value.ToString("dd-MMM-yyyy").ToString();
                }
                else
                {
                    ParamFromDate = "All";
                }

                if (dtpToDate.Checked)
                {
                    ParamToDate = dtpToDate.Value.ToString("dd-MMM-yyyy").ToString();
                }
                else
                {
                    ParamToDate = "All";
                }

                string[] ReportHeaders = new string[] {
                    Program.CompanyName
                     ,"Stock Details Movement","From Date:"+  ParamFromDate+"        To Date:"+  ParamToDate
                };


                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Stock");


                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    workSheet.Cells[i + 1, 1, (i + 1), (dt.Columns.Count)].Merge = true;
                    workSheet.Cells[i + 1, 1, (i + 1), (dt.Columns.Count)].Style.WrapText = true;
                    workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i]);
                }

                workSheet.Cells[ReportHeaders.Length + 2, 1].LoadFromDataTable(dt, true);


                int TableHeadRow = 0;
                TableHeadRow = ReportHeaders.Length + 2;

                int RowCount = 0;
                RowCount = dt.Rows.Count;

                int ColumnCount = 0;
                ColumnCount = dt.Columns.Count;

                int GrandTotalRow = 0;
                GrandTotalRow = TableHeadRow + RowCount + 1;

                int colNumber = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        workSheet.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {

                        workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        #region Grand Total
                        workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, colNumber].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                        #endregion
                    }

                }


                workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
                workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

                workSheet.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

                string p_strPath = Program.AppPath + @"\Excel Files\" + DateTime.Now.ToString("dd_MMM_yyyy_HH_mm_ss") + "_Stock_Report.xlsx";
                if (File.Exists(p_strPath))
                    File.Delete(p_strPath);

                FileStream objFileStrm = File.Create(p_strPath);
                objFileStrm.Close();

                //Write content to excel file    
                File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
                MessageBox.Show("File Save " + p_strPath + " Successfully");
                ProcessStartInfo psi = new ProcessStartInfo(p_strPath);
                psi.UseShellExecute = true;
                Process.Start(psi);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private void ExcelPreviewBackup()
        {
            try
            {
                DataSet Ds = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();

                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                dt1.TableName = "Copy";
                ReportDSDAL _dal = new ReportDSDAL();
                Ds = reportDsdal.StockNew(txtItemNo.Text.Trim(), txtPGroupId.Text.Trim(),
                                                      txtProductType.Text.Trim(),
                                                      vdtpStartDate, vdtpToDate, cmbPostText1, cmbPostText2, chkWithoutZero.Checked, chkCategoryLike.Checked, txtPGroup.Text.Trim(), 0, connVM);


                dt = Ds.Tables[0];
                dt1 = dt.Copy();

                string[] ReportHeaders = new string[] {
                    Program.CompanyName
                     ,"Stock "
                };


                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");


                for (int i = 0; i < ReportHeaders.Length; i++)
                {
                    workSheet.Cells[i + 1, 1, (i + 1), (dt.Columns.Count)].Merge = true;
                    workSheet.Cells[i + 1, 1, (i + 1), (dt.Columns.Count)].Style.WrapText = true;
                    workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i]);
                }
                string[] DeletecolumnName = new string[] { "Trading", "NonStock", "issueUCost", "TransDate", "TransType", "TransNumber" };
                dt = OrdinaryVATDesktop.DtDeleteColumns(dt, DeletecolumnName);

                //workSheet.Cells[dt.Rows.Count + ReportHeaders.Length + 3, 1].LoadFromText("Total");

                //int a = ReportHeaders.Length + 2;
                //int b = ReportHeaders.Length + dt.Rows.Count + 3;
                //int c = ReportHeaders.Length + dt.Rows.Count + 2;

                //workSheet.Cells["A" + (b) + ":" + OrdinaryVATDesktop.Alphabet[(dt.Columns.Count)] + (b)].Style.Font.Bold = true;
                //workSheet.Cells["A" + (a) + ":" + OrdinaryVATDesktop.Alphabet[(dt.Columns.Count)] + (a)].Style.Font.Bold = true;


                //workSheet.Cells["H" + (a + 1) + ":K" + (b)].Style.Numberformat.Format = @"_(* #,##0.0000000_);_(* (#,##0.0000000);_(* "" - ""??_);_(@_)";

                //workSheet.Cells["A" + (a) + ":" + OrdinaryVATDesktop.Alphabet[(dt.Columns.Count - 1)] + (b + 1)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //workSheet.Cells["A" + (a) + ":" + OrdinaryVATDesktop.Alphabet[(dt.Columns.Count)] + (b)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                workSheet.Cells[ReportHeaders.Length + 2, 1].LoadFromDataTable(dt, true);

                string p_strPath = Program.AppPath + @"\" + DateTime.Now.ToString("dd_MMM_yyyy_HH_mm_ss") + "_PriceDeclaration_Report.xlsx";
                if (File.Exists(p_strPath))
                    File.Delete(p_strPath);

                FileStream objFileStrm = File.Create(p_strPath);
                objFileStrm.Close();

                //Write content to excel file    
                File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
                MessageBox.Show("File Save " + p_strPath + " Successfully");
                ProcessStartInfo psi = new ProcessStartInfo(p_strPath);
                psi.UseShellExecute = true;
                Process.Start(psi);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void cmbPost_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbReportType_Leave(object sender, EventArgs e)
        {
            ReportType = cmbReportType.Text.Trim();
        }

        private void chkSummery_Click(object sender, EventArgs e)
        {
            chkStock.Visible = chkSummery.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExcelRpt = false;
            ExcelRptDetail = true;

            btnPreviewLoad();
        }

        private void cmbUOM_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                UOMToParam = cmbUOM.Text.Trim().ToLower();
                FindUOMConversuon();

                #endregion

            }

            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
            }
            #endregion Catch
        }

        private void cmbUOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                UOMToParam = cmbUOM.Text.Trim().ToLower();
                FindUOMConversuon();

                #endregion

            }

            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbUOM_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbUOM_Leave", exMessage);
            }
            #endregion Catch
        }

        private void rbtnItem_CheckedChanged(object sender, EventArgs e)
        {
            txtProductName.Enabled = rbtnItem.Checked;
            chkCategoryLike.Checked = false;
        }

        private void rbtnCategory_CheckedChanged(object sender, EventArgs e)
        {
            txtPGroup.Enabled = rbtnCategory.Checked;

        }

        private void rbtnType_CheckedChanged(object sender, EventArgs e)
        {
            cmbProductType.Enabled = rbtnType.Checked;
            chkCategoryLike.Checked = false;


        }



    }
}
