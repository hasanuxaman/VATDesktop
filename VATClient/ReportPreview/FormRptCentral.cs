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
using System.Threading.Tasks;
using OfficeOpenXml.Style;
using SymphonySofttech.Reports;
using VATServer.Interface;
using VATDesktop.Repo;


namespace VATClient.ReportPreview
{
    public partial class FormRptCentral : Form
    {
        public FormRptCentral()
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


        #region Global Variables For BackGroundWorker
        private ReportDocument reportDocument = new ReportDocument();

        private DataSet ReportResult = new DataSet();
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

        private async Task btnPreviewLoad()
        {
            try
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

                #endregion

                #region Report Date

                MISReport _report = new MISReport();
                _report.dtpFromDate = dtpFromDate.Checked;
                _report.dtpToDate = dtpToDate.Checked;


                #endregion

                #region Value Assign

                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                FormNumeric = cmbDecimal.Text.Trim();
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

                #endregion

                #region Report Control

                vm = new StockMovementVM
                {
                    ItemNo = itemNo,
                    StartDate = vdtpStartDate,
                    ToDate = vdtpToDate,
                    BranchId = BranchId,
                    Post1 = cmbPostText1,
                    Post2 = cmbPostText2,
                    ProductType = pType,
                    CategoryId = CategoryId,
                    ProductGroupName = gName,
                    CategoryLike = chkCategoryLike.Checked,
                    FormNumeric = FormNumeric,
                    CurrentUserID = Program.CurrentUserID,
                    CurrentUserName = Program.CurrentUser,
                    Branchwise = BranchId != 0
                };

                ExcelRpt = !chkSummery.Checked;
                ExcelRptDetail = chkSummery.Checked;
                string ParamFromDate = dtpFromDate.Checked ? dtpFromDate.Value.ToString("dd-MMM-yyyy").ToString() : "All";

                string ParamToDate = dtpToDate.Checked ? dtpToDate.Value.ToString("dd-MMM-yyyy").ToString() : "All";
                ReportDSDAL reportDsdal = new ReportDSDAL();

                DataTable dtResult = new DataTable();

                await Task.Run(() =>
                {
                    dtResult = reportDsdal.GetCentralData(vm);
                    ReportResult = new DataSet();
                    ReportResult.Tables.Add(dtResult);
                    ExcelPreview(ParamFromDate, ParamToDate);
                });


                #endregion

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }



        #region Background Worker Events

        private void NullCheck()
        {

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

                if (dtpFromDate.Checked == false)
                {
                    vdtpStartDate = "";

                    vdtpStartDate = dtpFromDate.MinDate.ToString("yyyy-MMM-dd");

                }
                else
                {
                    vdtpStartDate = dtpFromDate.Value.ToString("yyyy-MMM-dd").ToString();
                }
                if (dtpToDate.Checked == false)
                {
                    vdtpToDate = "";
                    vdtpToDate = dtpToDate.MaxDate.ToString("yyyy-MMM-dd");
                }
                else
                {
                    vdtpToDate = dtpToDate.Value.ToString("yyyy-MMM-dd").ToString();
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

        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {

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

                    ExcelPreviewDetails();
                }
                else
                {

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
            }
        }

        #endregion


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
        private void ExcelPreview(string ParamFromDate = "", string ParamToDate = "")
        {
            try
            {
                DataSet Ds = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();

                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                dt1.TableName = "Copy";

                dt = new DataTable();
                dt = ReportResult.Tables[0];

                dt1 = dt.Copy();

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
                    workSheet.Cells[i + 1, 1, (i + 1), (dt.Columns.Count)].Style.Font.Size = 14;
                    workSheet.Cells[i + 1, 1, (i + 1), (dt.Columns.Count)].Style.VerticalAlignment =
                        ExcelVerticalAlignment.Center;
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
                DataSet Ds = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();

                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                dt1.TableName = "Copy";

                dt = new DataTable();
                dt = ReportResult.Tables[1];

                string[] DeleteColumnName = { "ItemNo", "ItemNo1", "ClosingQuantityFinal", "ClosingAmountFinal" };

                dt = OrdinaryVATDesktop.DtDeleteColumns(dt, DeleteColumnName);

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

        private void chkSummery_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExcelRpt = false;
            ExcelRptDetail = true;

            btnPreviewLoad();
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
