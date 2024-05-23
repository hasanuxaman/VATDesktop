using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VATClient.ModelDTO;
using VATServer.Library;
using SymphonySofttech.Reports.Report;
using VATViewModel.DTOs;
using VATServer.License;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports;

namespace VATClient.ReportPreview
{
    public partial class FormRptSerialStockInformation : Form
    {
        List<ProductTypeDTO> ProductTypes = new List<ProductTypeDTO>();

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DataSet ReportResult;
        private string cmbProductTypeText;
        private DataTable ProductTypeResult;

        private string Heading1 = string.Empty;
        private string Heading2 = string.Empty;
        private string FromDate = string.Empty;
        private string ToDate = string.Empty;
        private string cmbPostText1 = string.Empty;
        private string cmbPostText2 = string.Empty;
        private ReportDocument reportDocument = new ReportDocument(); 


        public FormRptSerialStockInformation()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
			 
        }

			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            try
            {

                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//

                    txtPGroupId.Text = selectedRow.Cells["CategoryID"].Value.ToString();// ProductInfo[3];
                    txtCategoryName.Text = selectedRow.Cells["CategoryName"].Value.ToString();//ProductInfo[4];
                    cmbProductType.Text = selectedRow.Cells["IsRaw"].Value.ToString();//ProductInfo[9];

                }


            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }
            #endregion Catch
        }

        private void btnSearchGroup_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else 
                {
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtPGroupId.Text = ProductCategoryInfo[0];
                    txtCategoryName.Text = ProductCategoryInfo[1];
                    cmbProductType.Text = ProductCategoryInfo[4];
                }
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            #endregion Catch
        }

        private void FormRptSerialStockInformation_Load(object sender, EventArgs e)
        {
            #region Tracking
            CommonDAL commonDal = new CommonDAL();
            string vHeading1, vHeading2 = string.Empty;
            vHeading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1", null, connVM);
            vHeading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2", null, connVM);

            if (!string.IsNullOrEmpty(vHeading1) || !string.IsNullOrEmpty(vHeading2))
            {
                Heading1 = vHeading1;
                Heading2 = vHeading2;
            }

            #endregion

            cmbProductType.Items.Clear();

            ProductDAL productTypeDal = new ProductDAL();
            cmbProductType.DataSource = productTypeDal.ProductTypeList;
            cmbProductType.SelectedIndex = -1;

            #region Preview Only

            //CommonDAL commonDal = new CommonDAL();

            string PreviewOnly = commonDal.settingsDesktop("Reports", "PreviewOnly", null, connVM);

            if (PreviewOnly.ToLower() == "n")
            {
                cmbPost.Text = "Y";
                cmbPost.Visible = false;
                label1.Visible = false;
            }

            #endregion

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }
        private void ClearAllFields()
        {
            cmbProductType.SelectedIndex = -1;
            txtCategoryName.Text = "";
            cmbProductType.Text = "";
            txtItemNo.Text = "";
            txtProductName.Text = "";
            txtPGroupId.Text = "";
            dtpFromDate.Checked = false;
        }
        private void ReportShowDs()
        {
            try
            {
                NullCheck();

                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                cmbProductTypeText = cmbProductType.Text.Trim();
                backgroundWorkerPreview.RunWorkerAsync();

            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDs", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportShowDs", exMessage);
            }
            #endregion Catch
        }

        #region Background Worker Events
        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Try
            try
            {
                #region Start
                ReportResult = new DataSet();
                string itemNo = txtItemNo.Text.Trim();
                string productType = cmbProductTypeText;

               

                
                ReportDSDAL reportDsdal = new ReportDSDAL();
                ReportResult = reportDsdal.SerialStockStatus(itemNo, txtPGroupId.Text.Trim(), productType, FromDate, ToDate
                    ,cmbPostText1,cmbPostText2,connVM);


                #endregion
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               exMessage);
            }

            #endregion
        }
        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Try

            try
            {
                #region Complete

                //FormReport reports = new FormReport();
                //reports.crystalReportViewer1.Refresh();
                //reports.setReportSource(reportDocument);
                //reports.ShowDialog();






                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (ReportResult.Tables.Count <= 0 || ReportResult.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                ReportResult.Tables[0].TableName = "DsSerialStock";
                RptSerialStockStatus objrpt = new RptSerialStockStatus();
                objrpt.SetDataSource(ReportResult);
                
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Heading1"].Text = "'" + Heading1 + "'";
                objrpt.DataDefinition.FormulaFields["Heading2"].Text = "'" + Heading2 + "'";
               
                if (FromDate=="")
                {
                    objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'[All]'";
                    objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'-'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["FromDate"].Text = "'" +
                                                                            dtpFromDate.Value.ToString("dd/MMM/yyyy") +
                                                                            "'  ";
                }

                if (ToDate=="")
                {
                    objrpt.DataDefinition.FormulaFields["ToDate"].Text = "'-'";
                }
                else
                {
                    objrpt.DataDefinition.FormulaFields["ToDate"].Text = "'" +
                                                                          dtpToDate.Value.ToString("dd/MMM/yyyy") +
                                                                          "'  ";
                }

                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
             
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                
                FormReport reports = new FormReport(); 
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                #endregion
            }
            #endregion

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
            ReportShowDs();
        }

        private void NullCheck()
        {
            if (cmbPost.SelectedIndex != 0)
            {
                cmbPostText1 = "Y";
                cmbPostText2 = "N";
            }
            else
            {
                cmbPostText1 = cmbPost.Text.Trim();
                cmbPostText2 = cmbPost.Text.Trim();
            }
          

            if (dtpFromDate.Checked == false)
            {
                FromDate = "";
            }
            else
            {
                FromDate = dtpFromDate.Value.ToString("yyyy-MMM-dd");// dtpSalesFromDate.Value.ToString("yyyy-MMM-dd");
            }
            if (dtpToDate.Checked == false)
            {
                if (dtpFromDate.Checked == false)
                {
                    ToDate = "";
                }
                else
                {
                    ToDate = dtpToDate.Value.ToString("yyyy-MMM-dd");
                }
            }
            else
            {
                ToDate = dtpToDate.Value.ToString("yyyy-MMM-dd");// dtpSalesFromDate.Value.ToString("yyyy-MMM-dd");
            }


        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCategoryName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

    }
}
