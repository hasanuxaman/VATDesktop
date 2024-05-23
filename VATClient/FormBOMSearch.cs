using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using VATClient.ModelDTO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using VATServer.Library;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormBOMSearch : Form
    {
        #region Constructors

        public FormBOMSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        #endregion

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        string[] IdArray;
        string[] IdImportArray;
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        //public string VFIN = "315";
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        DataGridViewRow selectedRow = new DataGridViewRow();
        private string SelectedValue = string.Empty;
        private DataTable BOMResult;
        private string VatName = string.Empty;
        private string post = string.Empty;
        private static string TransectionType = string.Empty;
        private string searchBranchId = "0";
        private string ProductType = "";

        private string RecordCount = "0";
        private string EffectFromDate;
        private string EffectFromDateTo;
        private static bool IsDisposeFinishSearch = false;
        private static string ItemNo = null;

        #endregion
        private string vCustomerID = "0";
        private bool exportOverhead = false;

        public static DataGridViewRow SelectOne(string type, bool IsDisposeFinish = false, string itemNo = null)
        {
            DataGridViewRow selectedRowTemp = null;
            try
            {

                IsDisposeFinishSearch = IsDisposeFinish;
                ItemNo = itemNo;
                FormBOMSearch frmBOMSearch = new FormBOMSearch();
                TransectionType = type;
                if (type == "Service")
                {
                    frmBOMSearch.cmbVAT1Name.Items.Clear();

                    frmBOMSearch.cmbVAT1Name.Items.Add("Form Ka(Service)");
                    frmBOMSearch.cmbVAT1Name.SelectedIndex = 0;
                }
                else if (type == "Tender")
                {
                    frmBOMSearch.cmbVAT1Name.Items.Clear();

                    frmBOMSearch.cmbVAT1Name.Items.Add("VAT 4.3 (Tender)");
                    frmBOMSearch.cmbVAT1Name.SelectedIndex = 0;

                }

                frmBOMSearch.ShowDialog();
                //searcgValue = frmBOMSearch.SelectedValue;

                selectedRowTemp = frmBOMSearch.selectedRow;
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log("FormBOMSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormBOMSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log("FormBOMSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormBOMSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log("FormBOMSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, "FormBOMSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log("FormBOMSearch", "SelectOne", exMessage);
                MessageBox.Show(ex.Message, "FormBOMSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormBOMSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBOMSearch", "SelectOne", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "FormBOMSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBOMSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, "FormBOMSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBOMSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, "FormBOMSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormBOMSearch", "SelectOne", exMessage);
            }
            #endregion Catch

            return selectedRowTemp;
        }

        private void NullCheck()
        {

            try
            {
                VatName = string.Empty;
                if (dtpEffectDate.Checked == false)
                {
                    EffectFromDate = "";
                }
                else
                {
                    EffectFromDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");
                }

                if (dtpEffectDateTo.Checked == false)
                {
                    EffectFromDateTo = "";
                }
                else
                {
                    EffectFromDateTo = dtpEffectDateTo.Value.ToString("yyyy-MMM-dd");
                }


                VatName = cmbVAT1Name.Text.Trim();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "NullCheck", exMessage);
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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "NullCheck", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "NullCheck", exMessage);
            }
            #endregion Catch

        }

        #region Search Button Click, Method, Async Events

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RecordCount = cmbRecordCount.Text.Trim();
            searchBranchId = cmbBranch.SelectedValue.ToString();
            ProductType = cmbProductType.Text.Trim();
            Search();
        }

        private void Search()
        {
            NullCheck();
            //customerId();
            try
            {

                post = string.Empty;



                //if (cmbType.SelectedIndex != -1)
                //{
                //    VatName = cmbType.Text.Trim();
                //}

                post = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";

                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;
                bgwSearch.RunWorkerAsync();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Search", exMessage);
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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            #endregion Catch
        }

        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                //BOMDAL bomdal = new BOMDAL();
                IBOM bomdal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);

                //BOMResult = bomdal.SearchVAT1DTNew(txtProductName.Text.Trim(), EffectFromDate, VatName, post, txtCode.Text.Trim(), vCustomerID);

                string productName = OrdinaryVATDesktop.SanitizeInput(txtProductName.Text.Trim());
                string code = OrdinaryVATDesktop.SanitizeInput(txtCode.Text.Trim());

                string[] cValues = { productName, EffectFromDate, EffectFromDateTo, VatName, post, code, vCustomerID, searchBranchId, RecordCount, ProductType };
                string[] cFields = { "p.ProductName like", "bm.EffectDate>=", "bm.EffectDate<=", "bm.VATName ", "bm.Post like", "P.ProductCode", "bm.CustomerID ", "bm.BranchId", "SelectTop", "pc.IsRaw" };
                BOMResult = bomdal.SelectAll(null, cFields, cValues, null, null, null, true, connVM);

                string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

                BOMResult = OrdinaryVATDesktop.DtDeleteColumns(BOMResult, columnNames);

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
                FileLogger.Log(this.Name, "bgwSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string TotalTecordCount = "0";

            try
            {
                #region Statement

                dgvBOM.DataSource = null;
                if (BOMResult != null && BOMResult.Rows.Count > 0)
                {


                    TotalTecordCount = BOMResult.Rows[BOMResult.Rows.Count - 1][0].ToString();

                    BOMResult.Rows.RemoveAt(BOMResult.Rows.Count - 1);

                    dgvBOM.DataSource = BOMResult;

                    #region Specific Coloumns Visible False
                    dgvBOM.Columns["CustomerID"].Visible = false;
                    dgvBOM.Columns["CategoryID"].Visible = false;
                    dgvBOM.Columns["BranchId"].Visible = false;

                    #endregion
                }


                #region Comments Oct-01-2020


                //dgvBOM.Rows.Clear();
                //int j = 0;
                //foreach (DataRow item2 in BOMResult.Rows)
                //{
                //    DataGridViewRow NewRow = new DataGridViewRow();
                //    dgvBOM.Rows.Add(NewRow);
                //    dgvBOM.Rows[j].Cells["ReferenceNo"].Value = item2["ReferenceNo"].ToString();// Convert.ToDecimal(BOMFields[0]);


                //    dgvBOM.Rows[j].Cells["ItemNo"].Value = item2["FinishItemNo"].ToString();// Convert.ToDecimal(BOMFields[0]);
                //    dgvBOM.Rows[j].Cells["ProductName"].Value = item2["productname"].ToString();// BOMFields[1].ToString();
                //    dgvBOM.Rows[j].Cells["EffectDate"].Value = item2["EffectDate"].ToString();// BOMFields[2].ToString();
                //    dgvBOM.Rows[j].Cells["VATName"].Value = item2["VATName"].ToString();//BOMFields[3].ToString();
                //    dgvBOM.Rows[j].Cells["SalePrice"].Value = item2["NBRPrice"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["BOMId"].Value = item2["BOMId"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["ProductCode"].Value = item2["ProductCode"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["UOM"].Value = item2["UOM"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["HSCodeNo"].Value = item2["HSCodeNo"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["Comments"].Value = item2["Comments"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["SD"].Value = item2["SD"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["VATRate"].Value = item2["VATRate"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["TradingMarkUp"].Value = item2["TradingMarkUp"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["Post"].Value = item2["Post"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["CustomerID"].Value = item2["CustomerID"].ToString();// BOMFields[4].ToString();
                //    dgvBOM.Rows[j].Cells["CustomerName"].Value = item2["CustomerName"].ToString();
                //    dgvBOM.Rows[j].Cells["FirstSupplyDate"].Value = item2["FirstSupplyDate"].ToString();
                //    dgvBOM.Rows[j].Cells["BranchId"].Value = item2["BranchId"].ToString();

                //    j = j + 1;

                //}

                #endregion


                #endregion

            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {

                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
                LRecordCount.Text = "Total Record Count " + (dgvBOM.Rows.Count) + " of " + TotalTecordCount.ToString();

            }

        }

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void GridSelected()
        {
            try
            {
                if (dgvBOM.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvBOM.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }


                this.Close();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GridSelected", exMessage);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSelected", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GridSelected", exMessage);
            }
            #endregion Catch
        }

        private void dgvVendorGroup_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FormBOMSearch_Load(object sender, EventArgs e)
        {
            ProductDAL productTypeDal = new ProductDAL();

            cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;

            string[] Condition = new string[] { "ActiveStatus='Y'" };
            cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true);

            cmbBranch.SelectedValue = Program.BranchId;

            #region cmbBranch DropDownWidth Change
            string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
            cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
            #endregion

            //new FormAdjustmentSearch().BranchLoad(cmbBranch);
            VATName vname = new VATName();
            //cmbVAT1Name.DataSource = vname.VATNameList;
            //Search();
            //modify by ruba
            if (TransectionType != "Tender" && TransectionType != "Service")
            {
                cmbVAT1Name.DataSource = vname.VATNameList;
            }
            //customers();


            cmbProductType.DataSource = productTypeDal.ProductTypeListWithOutOverhead;
            cmbProductType.SelectedIndex = -1;



        }




        private void dgvBOM_DoubleClick(object sender, EventArgs e)
        {
            GridSelected();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Program.fromOpen = "Other";
                Program.R_F = "";


                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();//

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

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "button1_Click", exMessage);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                txtProductName.Text = "";
                txtItemNo.Text = "";
                cmbVAT1Name.SelectedIndex = 0;
                dtpEffectDate.Checked = false;
                dtpEffectDateTo.Checked = false;
                dgvBOM.Rows.Clear();
                cmbPost.SelectedIndex = -1;

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

        private void dtpEffectDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpEffectDateTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }


        private void cmbType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void chkCustCode_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkCustCode.Checked)
            //{
            //    chkCustCode.Text = "Code";
            //}
            //else
            //{
            //    chkCustCode.Text = "Name";
            //}

            //customerloadToCombo();
        }

        private void txtCustName_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtCustCode_TextChanged(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtCustomer_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                vCustomerID = "0";
                txtCustomer.Text = "";
                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormCustomerFinder.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    vCustomerID = selectedRow.Cells["CustomerID"].Value.ToString();
                    txtCustomer.Text = selectedRow.Cells["CustomerName"].Value.ToString();//ProductInfo[0];
                }
                txtCustomer.Focus();
                Search();
            }


        }

        private void btnCustomerRefresh_Click(object sender, EventArgs e)
        {
            vCustomerID = "0";
            txtCustomer.Text = "";
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            FormBOMEC frm = new FormBOMEC();
            frm.Show();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> BOMIdList = GetBomIdList().Select(x => x.BOMId).ToList();

                BOMDAL dal = new BOMDAL();

                DataTable data = dal.GetBOMExcelData(BOMIdList, null, null, connVM);

                data.Columns.Remove("Id");
                data.Columns.Remove("BOMId");
                data.Columns.Remove("RItemNo");
                data.Columns.Remove("Type");
                if (data.Rows.Count == 0)
                {
                    data.Rows.Add(data.NewRow());
                }

                OrdinaryVATDesktop.SaveExcel(data, "BOM", "Product");
                MessageBox.Show("Successfully Exported data in Excel files of root directory");

            }

            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);

            }

        }

        private List<BOMNBRVM> GetBomIdList()
        {
            List<BOMNBRVM> bomList = new List<BOMNBRVM>();

            int len = dgvBOM.Rows.Count;

            for (int i = 0; i < len; i++)
            {
                if (Convert.ToBoolean(dgvBOM["Select", i].Value)
                    //&& dgvSalesHistory["Post1", i].Value.ToString() != "Y"
                   )
                {
                    //ProductCode
                    bomList.Add(new BOMNBRVM()
                    {
                        BOMId = dgvBOM["BOMId", i].Value.ToString(),
                        FinishItemCode = dgvBOM["FinishItemNo", i].Value.ToString(),
                        EffectDate = dgvBOM["EffectDate", i].Value.ToString()
                    });
                }
            }

            return bomList;
        }

        private void btnPost_Click(object sender, EventArgs e)
        {

            #region Declarations

            int j = 0;
            string ids = "";

            #endregion

            #region Comments Nov-19-2020

            ////for (int i = 0; i < dgvBOM.Rows.Count; i++)
            ////{
            ////    if (Convert.ToBoolean(dgvBOM["Select", i].Value) == true)
            ////    {
            ////        if (dgvBOM["Post", i].Value.ToString() == "N")
            ////        {
            ////            string Id = dgvBOM["BOMId", i].Value.ToString();

            ////            ids += Id + "~";
            ////        }
            ////    }
            ////}

            #endregion

            #region Select GridView

            List<DataGridViewRow> dgvr = new List<DataGridViewRow>();
            dgvr = dgvBOM.Rows.Cast<DataGridViewRow>().Where(r => Convert.ToBoolean(r.Cells["Select"].Value) == true && Convert.ToString(r.Cells["Post"].Value) == "N").ToList();


            foreach (DataGridViewRow dr in dgvr)
            {

                string Id = dr.Cells["BOMId"].Value.ToString();

                ids += Id + "~";
            }

            #endregion

            #region Check Point

            IdArray = ids.Split('~');

            if (IdArray.Length <= 1)
            {
                MessageBox.Show(this, "All data already posted !");
                return;
            }

            #endregion

            #region Button States

            this.btnPost.Enabled = false;

            #endregion

            #region Background Worker

            bgwMultiplePost.RunWorkerAsync();

            #endregion

        }

        private void bgwMultiplePost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (IdArray == null || IdArray.Length <= 0)
                {
                    return;
                }

                if (
                    MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                BOMDAL bomdal = new BOMDAL();

                string[] results = bomdal.MultiplePost(IdArray, connVM);

                if (results[0].ToLower() == "success")
                {
                    MessageBox.Show("All BOM posted successfully");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgwMultiplePost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            #region Button States

            this.btnPost.Enabled = true;

            #endregion

            bgwSearch.RunWorkerAsync();
        }

        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                int rowsCount = dgvBOM.Rows.Count;
                for (int index = 0; index < rowsCount; index++)
                {
                    dgvBOM["Select", index].Value = chkSelect.Checked;
                }
            }
            catch (Exception exception)
            {
                FileLogger.Log("BOM Search", "chkSelect", exception.Message + "\n" + exception.StackTrace);
                MessageBox.Show(exception.Message);
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            try
            {
                List<BOMNBRVM> BOMList = GetBomIdList();

                if (BOMList.Count > 1 || BOMList.Count < 1)
                {
                    MessageBox.Show("Please Select only 1 Items");
                    return;
                }


                //List<BOMNBRVM> distinctBOMs = BOMList.GroupBy(b => b.FinishItemCode)
                //    .Select(g => g.First())
                //    .ToList(); // check for customer as well

                //if (distinctBOMs.Count != 1)
                //{
                //    MessageBox.Show("Selected Items Must be Same");
                //    return;
                //}

                exportOverhead = chkOverhead.Checked;
                progressBar1.Visible = true;
                progressBar1.Style = ProgressBarStyle.Marquee;
                bgwDownload.RunWorkerAsync(BOMList);

                ////MessageBox.Show("Successfully Exported data in Excel files of root directory");
                //ProcessStartInfo psi = new ProcessStartInfo(fileDirectory);
                //psi.UseShellExecute = true;
                //Process.Start(psi);
            }
            catch (Exception exception)
            {
                FileLogger.Log("BOM Search", "chkSelect", exception.Message + "\n" + exception.StackTrace);
                MessageBox.Show(exception.Message);
            }
        }

        private void bgwDownload_DoWork(object sender, DoWorkEventArgs e)
        {

            BOMDAL bomdal = new BOMDAL();
            List<BOMNBRVM> BOMList = (List<BOMNBRVM>)e.Argument;
            BOMNBRVM vm = bomdal.SelectPreviousBOM(BOMList.FirstOrDefault()).FirstOrDefault();

            if (vm == null)
            {
                throw new Exception("Can't find Previous BOM for Compare!");
            }

            BOMList.Add(new BOMNBRVM()
            {
                BOMId = vm.BOMId,
                FinishItemCode = vm.FinishItemNo,
                EffectDate = vm.EffectDate
            });
            DataTable dtResult = bomdal.GetCompareData(BOMList.Select(x => x.BOMId).ToList(), exportOverhead);
            ReportDSDAL reportDsdal = new ReportDSDAL();
            DataSet dsResult = reportDsdal.ComapnyProfile("", connVM);

            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            fileDirectory += "\\" + "BOM_Compare" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream fileStream = File.Create(fileDirectory);

            using (ExcelPackage package = new ExcelPackage(fileStream))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("BOMs");

                decimal PreviousCostTotala = 0;
                decimal CurrentCostTotala = 0;
                decimal value =  0;
                decimal Result = 0;

                string finishCode = dtResult.Rows[0]["FinishCode"].ToString();
                string finishName = dtResult.Rows[0]["FinishName"].ToString();
                string finishUOM = dtResult.Rows[0]["FinishUOM"].ToString();

                string PreviousEffectDate = dtResult.Rows[0]["FirstEffectDate"].ToString();
                string CurrentEffectDate = dtResult.Rows[0]["SecondEffectDate"].ToString();
                string InctiveDate = Convert.ToDateTime(dtResult.Rows[0]["SecondEffectDate"]).AddDays(-1).ToString();

                dtResult.Columns.Remove("FinishCode");
                dtResult.Columns.Remove("FinishName");
                dtResult.Columns.Remove("FirstEffectDate");
                dtResult.Columns.Remove("SecondEffectDate");
                dtResult.Columns.Remove("FinishUOM");


                ws.Cells[1, 1, 1, dtResult.Columns.Count].Merge = true;
                ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[1, 1, 1, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[2, 1, 2, dtResult.Columns.Count].Merge = true;
                ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[2, 1, 2, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[3, 1, 3, dtResult.Columns.Count].Merge = true;
                ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[3, 1, 3, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[4, 1, 4, dtResult.Columns.Count].Merge = true;
                ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[4, 1, 4, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[5, 1, 5, dtResult.Columns.Count].Merge = true;
                ws.Cells[5, 1, 5, dtResult.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[5, 1, 5, dtResult.Columns.Count].Style.Font.Size = 14;
                //ws.Cells[5, 1, 5, dtResult.Columns.Count].Style.Font.Bold = true;

                ws.Cells[7, 3, 7, 6].Merge = true;
                ws.Cells[7, 3, 7, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[7, 3, 7, 6].Style.Font.Size = 14;

                ws.Cells[7, 7, 7, 10].Merge = true;
                ws.Cells[7, 7, 7, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[7, 7, 7, 10].Style.Font.Size = 14;

                ws.Cells[7, 11, 7, 14].Merge = true;
                ws.Cells[7, 11, 7, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[7, 11, 7, 14].Style.Font.Size = 14;


                ws.Cells[1, 1].LoadFromText("Company Name: " + dsResult.Tables[0].Rows[0]["CompanyLegalName"]);
                ws.Cells[2, 1].LoadFromText("Address: " + dsResult.Tables[0].Rows[0]["Address1"]);

                ws.Cells[3, 1].LoadFromText("Product Name: " + finishName + " (" + finishCode + ")");
                ws.Cells[4, 1].LoadFromText("Previous Effect Date: " + PreviousEffectDate.ToDateString("dd-MMM-yyyy") + "      Inctive Date: " + InctiveDate.ToDateString("dd-MMM-yyyy"));
                ws.Cells[5, 1].LoadFromText("Current Effect Date: " + CurrentEffectDate.ToDateString("dd-MMM-yyyy"));

                ws.Cells[7, 3].LoadFromText("Previous_Effect Date: " + PreviousEffectDate.ToDateString("dd-MMM-yyyy"));
                ws.Cells[7, 7].LoadFromText("Current_Effect Date: " + CurrentEffectDate.ToDateString("dd-MMM-yyyy"));

                ws.Cells[7, 11].LoadFromText("Diff_Percentage");


                ws.Cells["A8"].LoadFromDataTable(dtResult, true);

                for (var index = 1; index <= dtResult.Columns.Count; index++)
                {
                    ws.Cells[8, index].Value = ws.Cells[8, index].Value.ToString().Replace("_Diff_Percentage", "")
                        .Replace("First_", "").Replace("Second_", "");
                }


                ws.Cells["A8:B" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["A8:B" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                ws.Cells["C8:F" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["C8:F" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);

                ws.Cells["G8:J" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["G8:J" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                ws.Cells["K8:N" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["K8:N" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                //ws.Cells["P1:S" + ws.Dimension.Rows].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //ws.Cells["P1:S" + ws.Dimension.Rows].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);
                ws.Cells[8 + dtResult.Rows.Count + 1, 6].Formula = "=Sum(" + ws.Cells[9, 6].Address + ":" + ws.Cells[8 + dtResult.Rows.Count, 6].Address + ")";
                ws.Cells[8 + dtResult.Rows.Count + 1, 10].Formula = "=Sum(" + ws.Cells[9, 10].Address + ":" + ws.Cells[8 + dtResult.Rows.Count, 10].Address + ")";

                if (dtResult.Rows.Count>0)
                {
                    PreviousCostTotala = Convert.ToDecimal(dtResult.Compute("SUM(First_Cost)", string.Empty));
                    CurrentCostTotala = Convert.ToDecimal(dtResult.Compute("SUM(Second_Cost)", string.Empty));
                     value = (100 /PreviousCostTotala) * CurrentCostTotala;
                     Result = (value - 100);
                }

                ws.Cells[8 + dtResult.Rows.Count + 1, 14].Value =  Convert.ToDecimal(Result.ToString("0.##"));
                decimal rate = 7.50M;
                if (Result >= rate)
                {
                    ws.Cells[8 + dtResult.Rows.Count + 1, 14].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    ws.Cells[8 + dtResult.Rows.Count + 1, 14].Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";

                }
                ws.Cells[8 + dtResult.Rows.Count + 1, 6].Style.Font.Bold = true;
                ws.Cells[8 + dtResult.Rows.Count + 1, 10].Style.Font.Bold = true;
                ws.Cells[8 + dtResult.Rows.Count + 1, 14].Style.Font.Bold = true;
               

                ws.Column(3).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(4).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(5).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(6).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(7).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(8).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(9).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(10).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";
                ws.Column(11).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";
                ws.Column(12).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";
                ws.Column(13).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";
                ws.Column(14).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";
                ws.Column(15).Style.Numberformat.Format = "#,##0.00\\%;[Red](#,##0.00\\%)";



                package.Save();
                fileStream.Close();
            }

            e.Result = fileDirectory;


        }

        private void bgwDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                    return;
                }

                ProcessStartInfo psi = new ProcessStartInfo((string)e.Result)
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }



    }
}