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
    public partial class Form6_1Process : Form
    {

        public Form6_1Process()
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
        VATRegistersDAL _vatRegistersDAL = new VATRegistersDAL();

        #endregion

        private void FormIssueMultiple_Load(object sender, EventArgs e)
        {
            //btnStartProcess.Visible = showReproc;
            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");
            string bigDataProcess = commonDal.settingValue("DayEnd", "BigDataProcess");


            txtProName.Enabled = chkProduct.Checked;
            dtpFromDate.Value = DateTime.Now;
            dtpToDate.Value = DateTime.Now;


            btnBranch.Visible = string.Equals(bigDataProcess, "Y", StringComparison.OrdinalIgnoreCase);
            btnBranchDay.Visible = string.Equals(bigDataProcess, "Y", StringComparison.OrdinalIgnoreCase);
            btnDay.Visible = string.Equals(bigDataProcess, "Y", StringComparison.OrdinalIgnoreCase);
        }

        private void ValidateFiscalPeriod(string periodStart, string periodEnd)
        {
            CommonDAL commonDal = new CommonDAL();
            DataTable dtResult = commonDal.FiscalYearLock(periodStart, periodEnd);

            if (dtResult.Rows.Count > 0)
            {
                List<ErrorMessage> errorMessages = dtResult.AsEnumerable().Select(row => new ErrorMessage() { ColumnName = row.Field<string>("PeriodName"), Message = "Period is Locked" }).ToList();

                FormErrorMessage.ShowDetails(errorMessages);

                throw new Exception("Please Unlock the Periods");
            }
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
            Process61();
        }

        private void Process61(bool isBranch = false)
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

                VAT6_1ParamVM vat61ParamVm = new VAT6_1ParamVM();
                if (!chkProduct.Checked)
                {
                    PeriodDetails(dtpDate.Value.ToString("MMyyyy"));
                }

                vat61ParamVm.StartDate = dtpFromDate.Value.ToString("dd-MMM-yyyy");
                vat61ParamVm.EndDate = dtpToDate.Value.ToString("dd-MMM-yyyy");

                vat61ParamVm.Post1 = "Y";
                vat61ParamVm.Post2 = "Y";
                vat61ParamVm.BranchId = 0;
                vat61ParamVm.PreviewOnly = false;
                vat61ParamVm.InEnglish = "N";
                vat61ParamVm.UOMConversion = 1;
                vat61ParamVm.UOM = "";
                vat61ParamVm.UOMTo = "";
                vat61ParamVm.UserName = Program.CurrentUser;
                vat61ParamVm.ReportName = "";
                vat61ParamVm.Opening = false;
                vat61ParamVm.OpeningFromProduct = false;

                vat61ParamVm.IsMonthly = false;
                vat61ParamVm.IsTopSheet = false;
                vat61ParamVm.IsGroupTopSheet = false;
                vat61ParamVm.Is6_1Permanent = true;
                vat61ParamVm.IsBranch = isBranch;

                vat61ParamVm.UserId = Program.CurrentUserID;

                if (chkProduct.Checked)
                {
                    vat61ParamVm.ItemNo = ItemNo;
                }


                ValidateFiscalPeriod(dtpFromDate.Value.ToString("dd-MMM-yyyy"),
                    dtpToDate.Value.AddDays(1).ToString("dd-MMM-yyyy"));

                bgwUpdate.RunWorkerAsync(vat61ParamVm);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                progressBar1.Visible = false;
            }
        }

        private void Process61_Day(bool isBranch = false)
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

                VAT6_1ParamVM vat61ParamVm = new VAT6_1ParamVM();
                if (!chkProduct.Checked)
                {
                    PeriodDetails(dtpDate.Value.ToString("MMyyyy"));
                }

                vat61ParamVm.StartDate = dtpFromDate.Value.ToString("dd-MMM-yyyy");
                vat61ParamVm.EndDate = dtpToDate.Value.ToString("dd-MMM-yyyy");

                vat61ParamVm.Post1 = "Y";
                vat61ParamVm.Post2 = "Y";
                vat61ParamVm.BranchId = 0;
                vat61ParamVm.PreviewOnly = false;
                vat61ParamVm.InEnglish = "N";
                vat61ParamVm.UOMConversion = 1;
                vat61ParamVm.UOM = "";
                vat61ParamVm.UOMTo = "";
                vat61ParamVm.UserName = Program.CurrentUser;
                vat61ParamVm.ReportName = "";
                vat61ParamVm.Opening = false;
                vat61ParamVm.OpeningFromProduct = false;

                vat61ParamVm.IsMonthly = false;
                vat61ParamVm.IsTopSheet = false;
                vat61ParamVm.IsGroupTopSheet = false;
                vat61ParamVm.Is6_1Permanent = true;
                vat61ParamVm.IsBranch = isBranch;

                vat61ParamVm.UserId = Program.CurrentUserID;

                if (chkProduct.Checked)
                {
                    vat61ParamVm.ItemNo = ItemNo;
                }


                ValidateFiscalPeriod(dtpFromDate.Value.ToString("dd-MMM-yyyy"),
                    dtpToDate.Value.AddDays(1).ToString("dd-MMM-yyyy"));

                bgwProcessDay.RunWorkerAsync(vat61ParamVm);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                progressBar1.Visible = false;
            }
        }


        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            IssueDAL issueDal = new IssueDAL();

            VAT6_1ParamVM varVAT6_1ParamVM = (VAT6_1ParamVM)e.Argument;
            string[] results = new string[6];

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("DayEnd", "BigDataProcess"); //commonDal.settingValue("CompanyCode", "Code");


            // Todo Seven Circle
            if (!string.Equals(code, "Y", StringComparison.OrdinalIgnoreCase))
            {
                results = _vatRegistersDAL.SaveVAT6_1_Permanent(varVAT6_1ParamVM, null, null, connVM);
                results = _vatRegistersDAL.SaveVAT6_1_Permanent_Branch(varVAT6_1ParamVM, null, null, connVM);
            }
            else
            {

                results = varVAT6_1ParamVM.IsBranch
                    ? issueDal.SaveVAT6_1_Permanent_Stored_Branch(varVAT6_1ParamVM, null, null, connVM)
                    : issueDal.SaveVAT6_1_Permanent_Stored(varVAT6_1ParamVM, null, null, connVM);
            }





            //results = issueDal.SaveVAT6_1_Permanent_DayWise(varVAT6_1ParamVM, null, null, connVM);
            //results = issueDal.SaveVAT6_1_Permanent_DayWise_Branch(varVAT6_1ParamVM, null, null, connVM);
            //results = issueDal.SaveVAT6_1_Permanent_Stored_Branch(varVAT6_1ParamVM, null, null, connVM);


            commonDal.settingsUpdateMaster("DayEnd", "DayEndProcess", "Y");
            e.Result = results;

        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    MessageBox.Show("Data Successfully Saved Permanently(6.1)  !");
                }
                else
                {
                    MessageBox.Show(e.Error.Message);
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                progressBar1.Visible = false;

            }

            #region Element Stats

            this.progressBar1.Visible = false;

            #endregion

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
where 1=1 and  p.ActiveStatus='Y' and p.ReportType = 'VAT6_1' ";



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
            try
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

                ValidateFiscalPeriod("", "");

                bgwDeleteProcess.RunWorkerAsync(ItemNo);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                progressBar1.Visible = false;

            }
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

        private void btnDay_Click(object sender, EventArgs e)
        {
            Process61_Day(true);
        }

        private void btnBranch_Click(object sender, EventArgs e)
        {
            Process61(true);
        }

        private void bgwProcessDay_DoWork(object sender, DoWorkEventArgs e)
        {
            IssueDAL issueDal = new IssueDAL();

            VAT6_1ParamVM varVAT6_1ParamVM = (VAT6_1ParamVM)e.Argument;
            string[] results = new string[6];

            CommonDAL commonDal = new CommonDAL();

            string code = commonDal.settingValue("CompanyCode", "Code");



            results = !varVAT6_1ParamVM.IsBranch
                ? issueDal.SaveVAT6_1_Permanent_DayWise(varVAT6_1ParamVM, null, null, connVM)
                : issueDal.SaveVAT6_1_Permanent_DayWise_Branch(varVAT6_1ParamVM, null, null, connVM);


            //results = issueDal.SaveVAT6_1_Permanent_DayWise(varVAT6_1ParamVM, null, null, connVM);
            //results = issueDal.SaveVAT6_1_Permanent_DayWise_Branch(varVAT6_1ParamVM, null, null, connVM);
            //results = issueDal.SaveVAT6_1_Permanent_Stored_Branch(varVAT6_1ParamVM, null, null, connVM);


            commonDal.settingsUpdateMaster("DayEnd", "DayEndProcess", "Y");
            e.Result = results;
        }

        private void bgwProcessDay_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    MessageBox.Show("Data Successfully Saved Permanently(6.1)  !");
                }
                else
                {
                    MessageBox.Show(e.Error.Message);
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                progressBar1.Visible = false;

            }

            #region Element Stats

            this.progressBar1.Visible = false;

            #endregion

        }

        private void btnBranchDay_Click(object sender, EventArgs e)
        {
            Process61_Day(true);
        }


    }
}
