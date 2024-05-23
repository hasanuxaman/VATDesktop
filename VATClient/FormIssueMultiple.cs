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
    public partial class FormIssueMultiple : Form
    {

        public FormIssueMultiple()
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
            btnStartProcess.Visible = showReproc;
            btnPermanent.Visible = showReproc;
            txtProName.Enabled = chkProduct.Checked;

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            this.progressBar1.Visible = true;


            IssueUpdate();
        }

        private void IssueUpdate()
        {

            try
            {

                #region Check Point

                ReceiveFromDate = dtpIssueFromDate.Value.ToString("yyyy-MMM-dd 00:00:00");
                //ReceiveToDate = dtpIssueToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");

                #endregion

                ValidateFiscalPeriod(ReceiveFromDate, "");

                bgwUpdate.RunWorkerAsync();

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

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
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

                priceVm.FullProcess = false;
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

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStartProcess_Click(object sender, EventArgs e)
        {
            try
            {
                this.progressBar1.Visible = true;

                AVGPriceVm vm = new AVGPriceVm();
                vm.AvgDateTime = ReceiveFromDate;

                if (!string.IsNullOrEmpty(ItemNo))
                {
                    vm.ItemNo = ItemNo;
                }

                ValidateFiscalPeriod("2019-07-01", "");


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
                IssueDAL issueDAl = new IssueDAL();

                AVGPriceVm vm = (AVGPriceVm)e.Argument;
                vm.AvgDateTime = String.Empty;
                vm.FullProcess = true;
                ResultVM rVM = issueDAl.UpdateAvgPrice_New_Refresh(vm);

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
                    // handle the error
                }
                else
                {
                    ResultVM result = (ResultVM)e.Result;
                    MessageBox.Show(result.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                this.progressBar1.Visible = false;

            }
        }

        private void btn6_1Process_Click(object sender, EventArgs e)
        {
            try
            {
                Form6_1Process form61Process = new Form6_1Process();
                form61Process.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btn6_2Process_Click(object sender, EventArgs e)
        {
            try
            {
                Form6_2Process form62Process = new Form6_2Process();
                form62Process.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
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

        private void txtProName_DoubleClick(object sender, EventArgs e)
        {
            ProductSelect();
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

            //if (!chkProduct.Checked)
            //{
            //    ItemNo = "";
            //    txtItemNo.Text = "";
            //    txtItemNo.Text = "";

            //}

        }

        private void btnPermanent_Click(object sender, EventArgs e)
        {
            try
            {
                // From frm = new FormProductItemSearch();
                FormPermanentProcess frm = new FormPermanentProcess();

                // frm.rbtnBankChannelPayment.Checked = true;
                frm.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRegular_Click(object sender, EventArgs e)
        {
            try
            {
                FormRegularProcess process = new FormRegularProcess();
                process.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


    }
}
