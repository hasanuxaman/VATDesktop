using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
////
//
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATDesktop.Repo;
using VATServer.Interface;
namespace VATClient
{
    public partial class FormStockProcess : Form
    {

        public FormStockProcess()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        #region Declarations

        IssueMasterVM vm = new IssueMasterVM();
        IssueDAL _IssueDAL = new IssueDAL();
        public string ItemNo { get; set; }

        string ReceiveFromDate = "";
        string ReceiveToDate = "";
        public bool showReproc = true;

        #endregion

        private void FormIssueMultiple_Load(object sender, EventArgs e)
        {
            //btnStartProcess.Visible = showReproc;

            txtProName.Enabled = chkProduct.Checked;
            dtpFromDate.Value = DateTime.Now;
            dtpToDate.Value = DateTime.Now;
        }
        private void PeriodDetails(string PeriodId)
        {
            try
            {
                FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();
                FiscalYearVM fiscalYearVM = new FiscalYearVM();

                string[] cFields = { "PeriodId" };
                string[] cValues = { PeriodId };

                fiscalYearVM = fiscalYearDAL.SelectAll(0, cFields, cValues, null, null, connVM).FirstOrDefault();
                if (fiscalYearVM == null)
                {
                    fiscalYearVM = fiscalYearDAL.SelectLastPeriod(0, null, null, null, null, connVM).FirstOrDefault();
                }
                dtpFromDate.Value = Convert.ToDateTime(fiscalYearVM.PeriodStart);
                dtpToDate.Value = Convert.ToDateTime(fiscalYearVM.PeriodEnd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string message = "Do you want to Process 6.1 Permanently?";
            string title = "Process 6.1";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.No)
            {
                return;
            }

            try
            {
                progressBar1.Visible = true;
                CommonDAL commonDal = new CommonDAL();
                string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);

                VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();
                if (!chkProduct.Checked)
                {
                    PeriodDetails(dtpDate.Value.ToString("MMyyyy"));
                }

                varVAT6_1ParamVM.StartDate = dtpFromDate.Value.ToString("dd-MMM-yyyy");
                varVAT6_1ParamVM.EndDate = dtpToDate.Value.ToString("dd-MMM-yyyy");

                varVAT6_1ParamVM.Post1 = "Y";
                varVAT6_1ParamVM.Post2 = "Y";
                varVAT6_1ParamVM.BranchId = 0;
                varVAT6_1ParamVM.PreviewOnly = false;
                varVAT6_1ParamVM.InEnglish = "N";
                varVAT6_1ParamVM.UOMConversion = 1;
                varVAT6_1ParamVM.UOM = "";
                varVAT6_1ParamVM.UOMTo = "";
                varVAT6_1ParamVM.UserName = Program.CurrentUser;
                varVAT6_1ParamVM.ReportName = "";
                varVAT6_1ParamVM.Opening = false;
                varVAT6_1ParamVM.OpeningFromProduct = false;

                varVAT6_1ParamVM.IsMonthly = false;
                varVAT6_1ParamVM.IsTopSheet = false;
                varVAT6_1ParamVM.IsGroupTopSheet = false;
                varVAT6_1ParamVM.Is6_1Permanent = true;

                varVAT6_1ParamVM.UserId = Program.CurrentUserID;

                if (chkProduct.Checked)
                {
                    varVAT6_1ParamVM.ItemNo = ItemNo;
                }

                bgwUpdate.RunWorkerAsync(varVAT6_1ParamVM);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

      

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            ProductDAL productDAl = new ProductDAL();
            ParameterVM vm = (ParameterVM)e.Argument;

            ResultVM rVM = productDAl.ProcessFreshStock(vm);
            //ResultVM rVM = productDAl.ProductRefresh(vm, null, null, Program.CurrentUserID, connVM);

            e.Result = rVM;
        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    ResultVM resultVM = (ResultVM)e.Result;
                    MessageBox.Show("Stock Process Completed");
                }
                else
                {
                    MessageBox.Show(e.Error.Message);

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                progressBar1.Visible = false;

            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStartProcess_Click(object sender, EventArgs e)
        {
            try
            {
  
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void txtProName_DoubleClick(object sender, EventArgs e)
        {
            ProductSelect();
        }

        private void ProductSelect()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.ShortName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
 left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y' and p.ReportType in( 'VAT6_1','VAT6_2','VAT6_1_And_6_2','VAT6_2_1') ";



                string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.ShortName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                string tableName = "";
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtProductCode.Clear();
                    txtProName.Clear();

                    txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();
                    ItemNo = selectedRow.Cells["ItemNo"].Value.ToString();

                    txtProductCode.Text = selectedRow.Cells["ProductCode"].Value.ToString();
                    txtProName.Text = selectedRow.Cells["ProductName"].Value.ToString();

                }



            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductGroupLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtProName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductSelect();
            }
        }

        private void chkProduct_CheckedChanged(object sender, EventArgs e)
        {
            txtProName.Enabled = chkProduct.Checked;
            dtpDate.Enabled = false;
            dtpFromDate.Enabled = false;
            dtpToDate.Enabled = false;

            if (chkProduct.Checked)
            {
                dtpDate.Enabled = false;
                dtpFromDate.Enabled = true;
                dtpToDate.Enabled = true;
            }
            else
            {
                dtpDate.Enabled = true;
                dtpFromDate.Enabled = false;
                dtpToDate.Enabled = false;
            }
            PeriodDetails(dtpDate.Value.ToString("MMyyyy"));

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DataLoad();
        }
        private void DataLoad()
        {
            //ProductDAL dal = new ProductDAL();
            //DataTable dt = dal.SelectNegInventoryData("6_1", null, null, null, null, connVM);
            //var dataSet = new DataSet();
            //dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);
            //dataSet.Tables.Add(dt);

            //var sheetNames = new[] { "VAT_6_1_Negetive" };

            //OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "VAT_6_1_Negetive", sheetNames);


            ProductDAL dal = new ProductDAL();
            DataTable dt = new DataTable();
            DataTable Branchdt = new DataTable();
            DataSet ds = dal.SelectNegInventoryData("6_1", null, null, null, null, connVM);

            var dataSet = new DataSet();
            dt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[0].Copy());
            dataSet.Tables.Add(dt);
            Branchdt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[1].Copy());
            dataSet.Tables.Add(Branchdt);

            var sheetNames = new[] { "VAT_6_1_Negetive", "VAT_6_1_BranchWiseNegetive" };

            OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "VAT_6_1_Negetive", sheetNames);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string message = "Do you want to delete 6.1 Permanent data?";
            string title = "Delete 6.1";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.No)
            {
                return;
            }
            progressBar1.Visible = true;

            bgwDeleteProcess.RunWorkerAsync(ItemNo);
        }

        private void bgwDeleteProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            ProductDAL productDal = new ProductDAL();
            string itemNo = (string)e.Argument;
            ResultVM vm = productDal.Delete6_1Permanent(itemNo); 
            vm = productDal.Delete6_1Permanent_Branch(itemNo);
        }

        private void bgwDeleteProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Error == null ? "Data has been deleted in VAT 6.1 permanent table!" : e.Error.Message);
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void dtpIssueFromDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnStockProcess_Click(object sender, EventArgs e)
        {
            try
            {
                ParameterVM vm = new ParameterVM();
                vm.ItemNo = ItemNo;
                vm.BranchId = Program.BranchId;
                vm.CurrentUserID = Program.CurrentUserID;
                vm.CurrentUser = Program.CurrentUser;

                progressBar1.Visible = true;
                bgwUpdate.RunWorkerAsync(vm);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
         

    }
}
