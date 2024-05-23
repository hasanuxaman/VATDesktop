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
using System.Diagnostics;
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
    public partial class Form6_2Process : Form
    {

        public Form6_2Process()
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

            CommonDAL commonDal = new CommonDAL();
            string code = commonDal.settingValue("DayEnd", "BigDataProcess"); //commonDal.settings("CompanyCode", "Code");

            btnBranchDay.Visible = code.ToLower() == "y";
            btnBranchProcess.Visible = code.ToLower() == "y";
            btnDayProcess.Visible = code.ToLower() == "y";
            btnBranchDay.Visible = code.ToLower() == "y";

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
                dtpFromDate.Value =Convert.ToDateTime( fiscalYearVM.PeriodStart);
                dtpToDate.Value = Convert.ToDateTime(fiscalYearVM.PeriodEnd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            RunVAT6_2Process();
        }

        private void RunVAT6_2Process(bool isBranch = false)
        {
            try
            {
                if (dtpFromDate.Value > dtpToDate.Value)
                {
                    MessageBox.Show("Date To must afer Date From");
                    return;
                }

                string message = "Do you want to Process 6.2 Permanently?";
                string title = "Process 6.2";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.No)
                {
                    return;
                }
                else
                {
                    // Do something  
                }

                progressBar1.Visible = true;
                CommonDAL commonDal = new CommonDAL();
                string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);

                VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();
                if (!chkProduct.Checked)
                {
                    PeriodDetails(dtpDate.Value.ToString("MMyyyy"));
                }

                varVAT6_2ParamVM.StartDate = dtpFromDate.Value.ToString("dd-MMM-yyyy 00:00:00");
                varVAT6_2ParamVM.EndDate = dtpToDate.Value.ToString("dd-MMM-yyyy 23:59:59");

                varVAT6_2ParamVM.Post1 = "Y";
                varVAT6_2ParamVM.Post2 = "Y";
                varVAT6_2ParamVM.BranchId = 0;
                varVAT6_2ParamVM.rbtnService = false;
                varVAT6_2ParamVM.rbtnWIP = false;
                varVAT6_2ParamVM.UOMTo = "";


                varVAT6_2ParamVM.IsBureau = Program.IsBureau;
                varVAT6_2ParamVM.AutoAdjustment = vAutoAdjustment == "Y";
                varVAT6_2ParamVM.PreviewOnly = false;
                varVAT6_2ParamVM.InEnglish = "N";

                varVAT6_2ParamVM.UOM = "";
                varVAT6_2ParamVM.IsMonthly = false;
                varVAT6_2ParamVM.IsTopSheet = false;

                varVAT6_2ParamVM.UserId = Program.CurrentUserID;
                varVAT6_2ParamVM.IsBranch = isBranch;

                if (!string.IsNullOrEmpty(ItemNo))
                {
                    varVAT6_2ParamVM.ItemNo = ItemNo;
                }

                ValidateFiscalPeriod(dtpFromDate.Value.ToString("dd-MMM-yyyy"),
                    dtpToDate.Value.AddDays(1).ToString("dd-MMM-yyyy"));


                bgwUpdate.RunWorkerAsync(varVAT6_2ParamVM);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                progressBar1.Visible = false;
            }
        }


        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            VATRegistersDAL _vATRegistersDAL = new VATRegistersDAL();
            
            VAT6_2ParamVM vat62ParamVm = (VAT6_2ParamVM)e.Argument;
            string[] results = new string[5];
            FileLogger.Log(this.Name, "Start time: ", DateTime.Now.ToString("HH:mm:ss"));


            CommonDAL commonDal = new CommonDAL();
            VATRegistersDAL _vatRegistersDAL = new VATRegistersDAL();

            string code = commonDal.settingValue("DayEnd", "BigDataProcess"); //commonDal.settings("CompanyCode", "Code");

            if (!string.Equals(code, "Y", StringComparison.OrdinalIgnoreCase) && !string.Equals(code, "smc", StringComparison.OrdinalIgnoreCase))
            {
                //vat62ParamVm.SkipOpening = true;
                results = _vatRegistersDAL.SaveVAT6_2_Permanent(vat62ParamVm, null, null, connVM);
                results = _vatRegistersDAL.SaveVAT6_2_Permanent_Branch(vat62ParamVm, null, null, connVM);

            }
            else
            {
                results = vat62ParamVm.IsBranch
                    ? _vATRegistersDAL.SaveVAT6_2_Permanent_Stored_Branch(vat62ParamVm, null, null, connVM)
                    : _vATRegistersDAL.SaveVAT6_2_Permanent_Stored(vat62ParamVm, null, null, connVM);

                //issueDal.SaveVAT6_2_Permanent_Stored_Branch(vat62ParamVm, null, null, connVM);
            }

            //foreach (DateTime dateTime in OrdinaryVATDesktop.EachDay(Convert.ToDateTime(vat62ParamVm.StartDate),
            //             Convert.ToDateTime(vat62ParamVm.EndDate)))
            //{

            //    vat62ParamVm.StartDate = dateTime.ToString("yyyy-MM-dd 00:00:00");
            //    vat62ParamVm.EndDate = dateTime.ToString("yyyy-MM-dd 23:59:59");

            //    //results = issueDal.SaveVAT6_2_Permanent(varVAT6_2ParamVM, null, null, connVM);

            //}

            //vat62ParamVm.StartDate = "2022-01-01";
            //vat62ParamVm.EndDate = "2022-01-01";

            //results = issueDal.SaveVAT6_2_Permanent(vat62ParamVm, null, null, connVM);

            e.Result = results;



            FileLogger.Log(this.Name, "End time: ", DateTime.Now.ToString("HH:mm:ss"));

        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                MessageBox.Show(e.Error == null ? "Data Successfully Saved Permanently(6.2)  ! " : e.Error.Message);


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

        private void txtProName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductSelect();
            }
        }

        private void ProductSelect()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.ShortName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
 left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y' and p.ReportType = 'VAT6_2' ";



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

        private void txtProName_DoubleClick(object sender, EventArgs e)
        {
            ProductSelect();
        }

        private void chkProduct_CheckedChanged(object sender, EventArgs e)
        {
            try
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
                ItemNo = "";
                PeriodDetails(dtpDate.Value.ToString("MMyyyy"));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }

        private void dtpIssueFromDate_ValueChanged(object sender, EventArgs e)
        {
            //DateTime newDateValue = new DateTime(dtpDate.Value.Year, 1, 1);
            //dtpDate.Value = newDateValue;
        }
        private void txtProName_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkProduct_Click(object sender, EventArgs e)
        {
            
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DataLoad();
        }
        private void DataLoad()
        {
           // ProductDAL dal = new ProductDAL();
           //DataTable dt = dal.SelectNegInventoryData("6_2", null, null, null,null, connVM);
           //var dataSet = new DataSet();
           //dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);
           // dataSet.Tables.Add(dt);

           // var sheetNames = new[] { "VAT_6_2_Negetive" };

           // OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "VAT_6_2_Negetive", sheetNames);

            ProductDAL dal = new ProductDAL();
            DataTable dt = new DataTable();
            DataTable Branchdt = new DataTable();
            DataSet ds = dal.SelectNegInventoryData("6_2", null, null, null, null, connVM);

            var dataSet = new DataSet();
            dt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[0].Copy());
            dataSet.Tables.Add(dt);
            Branchdt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[1].Copy());
            dataSet.Tables.Add(Branchdt);

            var sheetNames = new[] { "VAT_6_2_Negetive", "VAT_6_2_BranchWiseNegetive" };

            OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "VAT_6_2_Negetive", sheetNames);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string message = "Do you want to delete 6.2 Permanent data?";
                string title = "Delete 6.2";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.No)
                {
                    return;
                }
                progressBar1.Visible = true;

                ValidateFiscalPeriod("","");

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
            string itemNo = (string) e.Argument;
            ResultVM vm = productDal.Delete6_2Permanent(itemNo);
             vm = productDal.Delete6_2Permanent_Branch(itemNo);
             vm = productDal.Delete6_2Permanent_Branch_Day(itemNo);
             vm = productDal.Delete6_2Permanent_Day(itemNo);
        }

        private void bgwDeleteProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Error == null ? "Data has been deleted in VAT 6.2 permanent table!" : e.Error.Message);

            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void btnProcessSt_Click(object sender, EventArgs e)
        {

        }

        private void btnDayProcess_Click(object sender, EventArgs e)
        {
            VAT6_2_Branch_day();
        }

        private void VAT6_2_Branch_day(bool isBranch = false)
        {
            try
            {
                if (dtpFromDate.Value > dtpToDate.Value)
                {
                    MessageBox.Show("Date To must afer Date From");
                    return;
                }

                string message = "Do you want to Process 6.2 Daywise Permanently?";
                string title = "Process 6.2";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.No)
                {
                    return;
                }

                progressBar1.Visible = true;
                CommonDAL commonDal = new CommonDAL();
                string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);

                VAT6_2ParamVM paramVm = new VAT6_2ParamVM();
                if (!chkProduct.Checked)
                {
                    PeriodDetails(dtpDate.Value.ToString("MMyyyy"));
                }

                paramVm.StartDate = dtpFromDate.Value.ToString("dd-MMM-yyyy 00:00:00");
                paramVm.EndDate = dtpToDate.Value.ToString("dd-MMM-yyyy 23:59:59");

                paramVm.Post1 = "Y";
                paramVm.Post2 = "Y";
                paramVm.BranchId = 0;
                paramVm.rbtnService = false;
                paramVm.rbtnWIP = false;
                paramVm.UOMTo = "";


                paramVm.IsBureau = Program.IsBureau;
                paramVm.AutoAdjustment = vAutoAdjustment == "Y";
                paramVm.PreviewOnly = false;
                paramVm.InEnglish = "N";

                paramVm.UOM = "";
                paramVm.IsMonthly = false;
                paramVm.IsTopSheet = false;

                paramVm.UserId = Program.CurrentUserID;
                paramVm.IsBranch = isBranch;

                if (!string.IsNullOrEmpty(ItemNo))
                {
                    paramVm.ItemNo = ItemNo;
                }

                ValidateFiscalPeriod(dtpFromDate.Value.ToString("dd-MMM-yyyy"),
                    dtpToDate.Value.AddDays(1).ToString("dd-MMM-yyyy"));


                bgwProcessDay.RunWorkerAsync(paramVm);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                progressBar1.Visible = false;
            }
        }

        private void bgwProcessDay_DoWork(object sender, DoWorkEventArgs e)
        {
            VATRegistersDAL _vATRegistersDAL = new VATRegistersDAL();

            VAT6_2ParamVM vat62ParamVm = (VAT6_2ParamVM)e.Argument;
            string[] results = new string[5];
            FileLogger.Log(this.Name, "Start time: ", DateTime.Now.ToString("HH:mm:ss"));

            if (vat62ParamVm.IsBranch)
            {
                results = _vATRegistersDAL.SaveVAT6_2_Permanent_DayWise_Branch(vat62ParamVm, null, null, connVM);

            }
            else
            {
                results = _vATRegistersDAL.SaveVAT6_2_Permanent_DayWise(vat62ParamVm, null, null, connVM);
            }

            e.Result = results;



            FileLogger.Log(this.Name, "End time: ", DateTime.Now.ToString("HH:mm:ss"));
        }

        private void bgwProcessDay_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                MessageBox.Show(e.Error == null ? "Data Successfully Saved Permanently(6.2)  ! " : e.Error.Message);


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

        private void btnBranchProcess_Click(object sender, EventArgs e)
        {
            RunVAT6_2Process(true);
        }

        private void btnBranchDay_Click(object sender, EventArgs e)
        {
            VAT6_2_Branch_day(true);
        }

        private void dtpDate_KeyDown(object sender, KeyEventArgs e)
        {
            //var dpt = (DateTimePicker)sender;
            //DateTime newDateValue = new DateTime(dpt.Value.Year, dpt.Value.Month, 1);
            //dtpDate.Value = newDateValue;
        }
         
    }
}
