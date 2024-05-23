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
    public partial class FormPermanentProcess : Form
    {

        public FormPermanentProcess()
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
        private List<FiscalYearVM> fiscalYearVms = new List<FiscalYearVM>();
        #endregion

        private void FormIssueMultiple_Load(object sender, EventArgs e)
        {
            //btnStartProcess.Visible = showReproc;

            txtProName.Enabled = chkProduct.Checked;
            dtpFromDate.Value = DateTime.Now;
            dtpToDate.Value = DateTime.Now;
            PeriodDetails();

        }
        private void PeriodDetails()
        {
            try
            {
                FiscalYearDAL fiscalYearDAL = new FiscalYearDAL();
                FiscalYearVM fiscalYearVM = new FiscalYearVM();


                fiscalYearVms = fiscalYearDAL.SelectAll(0, null, null, null, null, connVM);
                fiscalYearVM = fiscalYearVms.FirstOrDefault();

                BindingSource bindingSource = new BindingSource { DataSource = fiscalYearVms };

                cmbFromMonth.DataSource = bindingSource;
                cmbFromMonth.DisplayMember = "PeriodName";
                cmbFromMonth.ValueMember = "PeriodName";


                BindingSource TobindingSource = new BindingSource { DataSource = fiscalYearVms };

                cmbToMonth.DataSource = TobindingSource;
                cmbToMonth.DisplayMember = "PeriodName";
                cmbToMonth.ValueMember = "PeriodName";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            try
            {

                if (rbtn6_1.Checked)
                {
                    VAT6_1Process();
                }
                else if (rbtn6_2.Checked)
                {
                    VAT6_2Process();
                }
                else if (rbtn6_2_1.Checked)
                {
                    VAT6_2_1Process();
                }
                else if (rtnBoth.Checked)
                {
                    BothSaveProcess();
                }
                else if (rbtn6_1Neg.Checked)
                {
                    var sheetNames = new[] { "VAT_6_1_Negetive" };

                    DataLoad("6_1", "VAT_6_1_Negetive", sheetNames);
                }
                else if (rbtn6_2Neg.Checked)
                {
                    var sheetNames = new[] { "VAT_6_2_Negetive" };

                    DataLoad("6_2", "VAT_6_2_Negetive", sheetNames);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void VAT6_1Process()
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

                VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();

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

                varVAT6_1ParamVM.FromPeriodName = cmbFromMonth.Text;
                varVAT6_1ParamVM.ToPeriodName = cmbToMonth.Text;

                varVAT6_1ParamVM.FromSP = false;

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

            VAT6_1ParamVM varVAT6_1ParamVM = (VAT6_1ParamVM)e.Argument;

            FiscalYearVM fromVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_1ParamVM.FromPeriodName);
            FiscalYearVM toVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_1ParamVM.ToPeriodName);

            var results = VAT6_1RangeProcess(fromVm, toVm, varVAT6_1ParamVM);

            e.Result = results;


        }

        private string[] VAT6_1RangeProcess(FiscalYearVM fromVm, FiscalYearVM toVm, VAT6_1ParamVM varVAT6_1ParamVM)
        {
            IssueDAL issueDal = new IssueDAL();
            string[] results = new string[5];
            string bigProcess = new CommonDAL().settingValue("DayEnd", "BigDataProcess");
            if (fromVm == null || toVm == null)
            {
                throw new Exception("Select Fiscal Range Not Found");
            }

            List<FiscalYearVM> filteredYears = fiscalYearVms.Where(x =>
                Convert.ToDateTime(x.PeriodStart) >= Convert.ToDateTime(fromVm.PeriodStart) &&
                Convert.ToDateTime(x.PeriodStart) <= Convert.ToDateTime(toVm.PeriodStart)).ToList();

            ValidateFiscalPeriod(fromVm.PeriodStart, toVm.PeriodStart);

            CommonDAL commonDal = new CommonDAL();
            bool IsFromSP = commonDal.settings("VAT6_1", "FromSP", null, null, connVM) == "Y";

            varVAT6_1ParamVM.FromSP = IsFromSP;

            foreach (FiscalYearVM filteredYear in filteredYears)
            {
                varVAT6_1ParamVM.StartDate = filteredYear.PeriodStart.ToDateString();
                varVAT6_1ParamVM.EndDate = filteredYear.PeriodEnd.ToDateString();
                varVAT6_1ParamVM.BranchId = 0;
                ////results = _vatRegistersDAL.SaveVAT6_1_Permanent(varVAT6_1ParamVM, null, null, connVM);
                ////results = _vatRegistersDAL.SaveVAT6_1_Permanent_Branch(varVAT6_1ParamVM, null, null, connVM);


                if (bigProcess.ToLower() == "y")
                {
                    issueDal.SaveVAT6_1_Permanent_Stored(varVAT6_1ParamVM, null, null, connVM);
                    issueDal.SaveVAT6_1_Permanent_Stored_Branch(varVAT6_1ParamVM, null, null, connVM);

                    results = issueDal.SaveVAT6_1_Permanent_DayWise(varVAT6_1ParamVM, null, null, connVM);
                    results = issueDal.SaveVAT6_1_Permanent_DayWise_Branch(varVAT6_1ParamVM, null, null, connVM);

                }
                else
                {
                    results = _vatRegistersDAL.SaveVAT6_1_Permanent(varVAT6_1ParamVM, null, null, connVM);
                    results = _vatRegistersDAL.SaveVAT6_1_Permanent_Branch(varVAT6_1ParamVM, null, null, connVM);
                }
            }

            return results;
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
                    FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", e.Error.ToString());
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
where 1=1 and  p.ActiveStatus='Y' and p.ReportType in ('VAT6_1','VAT6_2', 'VAT6_2_1','VAT6_1_And_6_2') ";



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
            ItemNo = "";
            //dtpDate.Enabled = false;
            //dtpFromDate.Enabled = false;
            //dtpToDate.Enabled = false;

            //if (chkProduct.Checked)
            //{
            //    dtpDate.Enabled = false;
            //    dtpFromDate.Enabled = true;
            //    dtpToDate.Enabled = true;
            //}
            //else
            //{
            //    dtpDate.Enabled = true;
            //    dtpFromDate.Enabled = false;
            //    dtpToDate.Enabled = false;
            //}
            //PeriodDetails(dtpDate.Value.ToString("MMyyyy"));

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            var sheetNames = new[] { "VAT_6_1_Negetive", "VAT_6_1_BranchWiseNegetive" };

            DataLoad("VAT6_1", "VAT_6_1_Negetive", sheetNames);
        }
        private void DataLoad(string VATType, string name, string[] sheetNames)
        {
            //ProductDAL dal = new ProductDAL();
            //DataTable dt = dal.SelectNegInventoryData(VATType, null, null, null, null, connVM);
            //var dataSet = new DataSet();
            //dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);
            //dataSet.Tables.Add(dt);


            //OrdinaryVATDesktop.SaveExcelMultiple(dataSet, name, sheetNames);

            Cursor.Current = Cursors.WaitCursor;
            ProductDAL dal = new ProductDAL();
            DataTable dt = new DataTable();
            DataTable Branchdt = new DataTable();
            DataSet ds = dal.SelectNegInventoryData("6_1", null, null, null, null, connVM);

            var dataSet = new DataSet();
            dt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[0]);
            Branchdt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[1]);
            dataSet.Tables.Add(dt.Copy());
            dataSet.Tables.Add(Branchdt.Copy());


            OrdinaryVATDesktop.SaveExcelMultiple(dataSet, name, sheetNames);

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateFiscalPeriod("", "");

                if (rbtn6_1Delete.Checked)
                {
                    VAT6_1Delete();
                }
                else if (rbtn6_2Delete.Checked)
                {
                    VAT6_2Delete();
                }
                else if (rbtn6_2_1del.Checked)
                {
                    VAT6_2_1Delete();
                }
                else if (rbtnBothDelete.Checked)
                {
                    BothDeleteProcess();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void VAT6_1Delete()
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

        private void btn6_2Process_Click(object sender, EventArgs e)
        {
            VAT6_2Process();
        }

        private void VAT6_2Process()
        {
            try
            {
                string message = "Do you want to Process 6.2 Permanently?";
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

                VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();


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

                varVAT6_2ParamVM.FromSP = false;

                varVAT6_2ParamVM.UserId = Program.CurrentUserID;

                varVAT6_2ParamVM.FromPeriodName = cmbFromMonth.Text;
                varVAT6_2ParamVM.ToPeriodName = cmbToMonth.Text;

                if (!string.IsNullOrEmpty(ItemNo))
                {
                    varVAT6_2ParamVM.ItemNo = ItemNo;
                }

                bgw6_2Process.RunWorkerAsync(varVAT6_2ParamVM);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void VAT6_2_1Process()
        {
            try
            {
                string message = "Do you want to Process 6.2.1 Permanently?";
                string title = "Process 6.2.1";
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

                paramVm.FromPeriodName = cmbFromMonth.Text;
                paramVm.ToPeriodName = cmbToMonth.Text;
                paramVm.FromSP = true;
                if (!string.IsNullOrEmpty(ItemNo))
                {
                    paramVm.ItemNo = ItemNo;
                }

                bgwVAT6_2_1Process.RunWorkerAsync(paramVm);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void bgw6_2Process_DoWork(object sender, DoWorkEventArgs e)
        {

            VAT6_2ParamVM varVAT6_2ParamVM = (VAT6_2ParamVM)e.Argument;

            FiscalYearVM fromVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_2ParamVM.FromPeriodName);
            FiscalYearVM toVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == varVAT6_2ParamVM.ToPeriodName);

            var results = VAT6_2RangeProcess(fromVm, toVm, varVAT6_2ParamVM);

            e.Result = results;

        }

        private string[] VAT6_2RangeProcess(FiscalYearVM fromVm, FiscalYearVM toVm, VAT6_2ParamVM varVAT6_2ParamVM)
        {
            VATRegistersDAL _vATRegistersDAL = new VATRegistersDAL();
            string bigProcess = new CommonDAL().settingValue("DayEnd", "BigDataProcess");

            if (fromVm == null || toVm == null)
            {
                throw new Exception("Select Fiscal Range Not Found");
            }


            List<FiscalYearVM> filteredYears = fiscalYearVms.Where(x =>
                Convert.ToDateTime(x.PeriodStart) >= Convert.ToDateTime(fromVm.PeriodStart) &&
                Convert.ToDateTime(x.PeriodStart) <= Convert.ToDateTime(toVm.PeriodStart)).ToList();

            string[] results = new string[5];

            ValidateFiscalPeriod(fromVm.PeriodStart, toVm.PeriodStart);

            CommonDAL commonDal = new CommonDAL();
            bool IsFromSP = commonDal.settings("VAT6_2", "FromSP", null, null, connVM) == "Y";
            varVAT6_2ParamVM.FromSP = IsFromSP;

            foreach (FiscalYearVM filteredYear in filteredYears)
            {
                varVAT6_2ParamVM.StartDate = filteredYear.PeriodStart.ToDateString();
                varVAT6_2ParamVM.EndDate = filteredYear.PeriodEnd.ToDateString();
                varVAT6_2ParamVM.BranchId = 0;
                varVAT6_2ParamVM.PeriodId = Convert.ToDateTime(varVAT6_2ParamVM.StartDate).ToString("MMyyyy");

                if (bigProcess.ToLower() == "y")
                {
                    _vATRegistersDAL.SaveVAT6_2_Permanent_Stored(varVAT6_2ParamVM, null, null, connVM);
                    _vATRegistersDAL.SaveVAT6_2_Permanent_Stored_Branch(varVAT6_2ParamVM, null, null, connVM);

                    results = _vATRegistersDAL.SaveVAT6_2_Permanent_DayWise(varVAT6_2ParamVM, null, null, connVM);
                    results = _vATRegistersDAL.SaveVAT6_2_Permanent_DayWise_Branch(varVAT6_2ParamVM, null, null, connVM);

                }
                else
                {

                    results = _vATRegistersDAL.SaveVAT6_2_Permanent(varVAT6_2ParamVM, null, null, connVM);
                    results = _vATRegistersDAL.SaveVAT6_2_Permanent_Branch(varVAT6_2ParamVM, null, null, connVM);
                }

            }

            return results;
        }

        private string[] VAT6_2_1RangeProcess(FiscalYearVM fromVm, FiscalYearVM toVm, VAT6_2ParamVM varVAT6_2ParamVM)
        {
            IssueDAL issueDal = new IssueDAL();

            if (fromVm == null || toVm == null)
            {
                throw new Exception("Select Fiscal Range Not Found");
            }

            List<FiscalYearVM> filteredYears = fiscalYearVms.Where(x =>
                Convert.ToDateTime(x.PeriodStart) >= Convert.ToDateTime(fromVm.PeriodStart) &&
                Convert.ToDateTime(x.PeriodStart) <= Convert.ToDateTime(toVm.PeriodStart)).ToList();

            string[] results = new string[5];

            ValidateFiscalPeriod(fromVm.PeriodStart, toVm.PeriodStart);
            if (false)
            {


                foreach (FiscalYearVM filteredYear in filteredYears)
                {
                    varVAT6_2ParamVM.StartDate = filteredYear.PeriodStart.ToDateString();
                    varVAT6_2ParamVM.EndDate = filteredYear.PeriodEnd.ToDateString();

                    results = issueDal.SaveVAT6_2_1_Permanent(varVAT6_2ParamVM, null, null, connVM);
                    //results = issueDal.SaveVAT6_2_1_Permanent_Branch(varVAT6_2ParamVM, null, null, connVM);

                }
            }
            foreach (FiscalYearVM filteredYear in filteredYears)
            {
                varVAT6_2ParamVM.StartDate = filteredYear.PeriodStart.ToDateString();
                varVAT6_2ParamVM.EndDate = filteredYear.PeriodEnd.ToDateString();
                results = _vatRegistersDAL.SaveVAT6_2_1_Permanent(varVAT6_2ParamVM, null, null, connVM);
                results = _vatRegistersDAL.SaveVAT6_2_1_Permanent_Branch(varVAT6_2ParamVM, null, null, connVM);

            }

            return results;
        }

        private void bgw6_2Process_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    MessageBox.Show("Data Successfully Saved Permanently(6.2)  !");
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

        private void btnBoth_Click(object sender, EventArgs e)
        {
            BothSaveProcess();
        }

        private void BothSaveProcess()
        {
            try
            {
                CommonDAL commonDal = new CommonDAL();
                string vAutoAdjustment = commonDal.settingsDesktop("VAT6_2", "AutoAdjustment", null, connVM);

                VAT6_2ParamVM varVAT6_2ParamVM = new VAT6_2ParamVM();


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

                varVAT6_2ParamVM.FromPeriodName = cmbFromMonth.Text;
                varVAT6_2ParamVM.ToPeriodName = cmbToMonth.Text;

                if (!string.IsNullOrEmpty(ItemNo))
                {
                    varVAT6_2ParamVM.ItemNo = ItemNo;
                }

                VAT6_1ParamVM varVAT6_1ParamVM = new VAT6_1ParamVM();

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

                varVAT6_1ParamVM.FromPeriodName = cmbFromMonth.Text;
                varVAT6_1ParamVM.ToPeriodName = cmbToMonth.Text;

                if (chkProduct.Checked)
                {
                    varVAT6_1ParamVM.ItemNo = ItemNo;
                }

                List<Object> arguments = new List<object> { varVAT6_1ParamVM, varVAT6_2ParamVM };

                progressBar1.Visible = true;
                bgwBothProcess.RunWorkerAsync(arguments);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void bgwBothProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            var arguments = e.Argument as List<Object>;

            VAT6_1ParamVM VAT6_1vm = (VAT6_1ParamVM)arguments[0];
            VAT6_2ParamVM VAT6_2vm = (VAT6_2ParamVM)arguments[1];


            FiscalYearVM fromVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == VAT6_2vm.FromPeriodName);
            FiscalYearVM toVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == VAT6_2vm.ToPeriodName);

            string[] res1 = VAT6_1RangeProcess(fromVm, toVm, VAT6_1vm);
            string[] res2 = VAT6_2RangeProcess(fromVm, toVm, VAT6_2vm);
        }

        private void bgwBothProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    MessageBox.Show("Data Successfully Saved Permanently(6.1 & 6.2)  !");
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

        private void btn6_2delete_Click(object sender, EventArgs e)
        {
            VAT6_2Delete();
        }

        private void VAT6_2Delete()
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

            bgwVAT6_2delete.RunWorkerAsync(ItemNo);
        }
        private void VAT6_2_1Delete()
        {
            string message = "Do you want to delete 6.2.1 Permanent data?";
            string title = "Delete 6.2.1";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.No)
            {
                return;
            }

            progressBar1.Visible = true;

            bgw6_2_1.RunWorkerAsync(ItemNo);
        }
        private void btnBothDelete_Click(object sender, EventArgs e)
        {
            BothDeleteProcess();
        }

        private void BothDeleteProcess()
        {
            string message = "Do you want to delete 6.1 & 6.2 Permanent data?";
            string title = "Delete 6.1 & 6.2";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.No)
            {
                return;
            }

            progressBar1.Visible = true;

            bgwBothDelete.RunWorkerAsync(ItemNo);
        }

        private void bgwVAT6_2delete_DoWork(object sender, DoWorkEventArgs e)
        {
            ProductDAL productDal = new ProductDAL();
            string itemNo = (string)e.Argument;
            ResultVM vm = productDal.Delete6_2Permanent(itemNo);
            vm = productDal.Delete6_2Permanent_Branch(itemNo);
        }

        private void bgwVAT6_2delete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        private void bgwBothDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            ProductDAL productDal = new ProductDAL();
            string itemNo = (string)e.Argument;

            ResultVM vm = productDal.Delete6_2Permanent(itemNo);
            vm = productDal.Delete6_2Permanent_Branch(itemNo);

            vm = productDal.Delete6_1Permanent(itemNo);
            vm = productDal.Delete6_1Permanent_Branch(itemNo);
        }

        private void bgwBothDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Error == null ? "Data has been deleted in VAT 6.1 & 6.2 permanent table!" : e.Error.Message);
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void btnNegativeDownload_Click(object sender, EventArgs e)
        {

            var sheetNames = new[] { "VAT_6_1_Negetive" };

            DataLoad("6_1", "VAT_6_1_Negetive", sheetNames);
        }

        private void btnNeg6_2_Click(object sender, EventArgs e)
        {

            var sheetNames = new[] { "VAT_6_2_Negetive" };

            DataLoad("6_2", "VAT_6_2_Negetive", sheetNames);
        }

        private void bgwVAT6_2_1_DoWork(object sender, DoWorkEventArgs e)
        {
            VAT6_2ParamVM argument = (VAT6_2ParamVM)e.Argument;

            FiscalYearVM fromVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == argument.FromPeriodName);
            FiscalYearVM toVm = fiscalYearVms.FirstOrDefault(x => x.PeriodName == argument.ToPeriodName);

            string[] results = VAT6_2_1RangeProcess(fromVm, toVm, argument);

            e.Result = results;
        }

        private void bgwVAT6_2_1Process_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Error == null ? "Data Successfully Saved Permanently(6.2.1)  !" : e.Error.Message);
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

        private void bgw6_2_1_DoWork(object sender, DoWorkEventArgs e)
        {
            ProductDAL productDal = new ProductDAL();
            string itemNo = (string)e.Argument;
            ResultVM vm = productDal.Delete6_2_1Permanent(itemNo);
            vm = productDal.Delete6_2_1Permanent_Branch(itemNo);
        }

        private void bgw6_2_1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Error == null ? "Data has been deleted in VAT 6.2.1 permanent table!" : e.Error.Message);
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {

            IssueUpdate();
        }

        private void IssueUpdate()
        {

            try
            {

                #region Check Point

                ReceiveFromDate = dtpIssueFromDate.Value.ToString("yyyy-MMM-dd 00:00:00");

                #endregion

                try
                {

                    ValidateFiscalPeriod(ReceiveFromDate, "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }


                this.progressBar1.Visible = true;

                bgwAvg.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "Update", exMessage);
                this.progressBar1.Visible = false;

            }
            #endregion

            finally { }


        }

        private void bgwAvg_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ResultVM rVM = new ResultVM();


                AVGPriceVm priceVm = new AVGPriceVm
                {
                    AvgDateTime = ReceiveFromDate
                };

                if (!string.IsNullOrEmpty(ItemNo))
                {
                    priceVm.ItemNo = ItemNo;
                }

                rVM = _IssueDAL.UpdateAvgPrice_New_Refresh(priceVm);

                e.Result = rVM;
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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            #endregion

            finally { }
        }

        private void bgwAvg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                if (e.Error == null)
                {
                    ResultVM rVM = (ResultVM)e.Result;

                    if (rVM != null)
                    {
                        MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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

            finally { }

            #region Element Stats

            this.progressBar1.Visible = false;

            #endregion

        }

        private void btnReProcess_Click(object sender, EventArgs e)
        {
            try
            {

                AVGPriceVm vm = new AVGPriceVm();
                vm.AvgDateTime = ReceiveFromDate;
                vm.FromSP = false;
                if (!string.IsNullOrEmpty(ItemNo))
                {
                    vm.ItemNo = ItemNo;
                }

                try
                {
                    ValidateFiscalPeriod("2019-07-01", "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                this.progressBar1.Visible = true;

                bgwRefresh.RunWorkerAsync(vm);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void bgwRefresh_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                AVGPriceVm vm = (AVGPriceVm)e.Argument;
                vm.AvgDateTime = String.Empty;
                ResultVM rVM = _vatRegistersDAL.UpdateAvgPrice_New_Refresh(vm);

                e.Result = rVM;
            }
            catch (Exception ex)
            {
                e.Result = new ResultVM();
            }
        }

        private void bgwRefresh_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                }
                else
                {
                    ResultVM result = (ResultVM)e.Result;
                    MessageBox.Show(result.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            finally
            {
                this.progressBar1.Visible = false;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //FormStockProcess formStockProcess = new FormStockProcess();
                //formStockProcess.ShowDialog();

                ResultVM rVM = new ResultVM();


                ParameterVM vm = new ParameterVM();
                vm.FromSP = true;
                vm.ItemNo = ItemNo;
                vm.BranchId = Program.BranchId;
                vm.CurrentUserID = Program.CurrentUserID;
                vm.CurrentUser = Program.CurrentUser;

                progressBar1.Visible = true;

                bgwFreshStock.RunWorkerAsync(vm);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btnDbAll_Click(object sender, EventArgs e)
        {
            FormDBMigration frm = new FormDBMigration();
            frm.Show();
        }

        private void rbtn6_2_1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void bgwFreshStock_DoWork(object sender, DoWorkEventArgs e)
        {
            ParameterVM vm = (ParameterVM)e.Argument;
            ResultVM rVM = _vatRegistersDAL.ProcessFreshStock(vm);

            e.Result = rVM;
        }

        private void bgwFreshStock_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
    }
}
