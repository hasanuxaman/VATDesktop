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
    public partial class FormPurchaseMultiples : Form
    {
        #region Declarations
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        PurchaseMasterVM vm = new PurchaseMasterVM();
        PurchaseDAL _purchaseDAL = new PurchaseDAL();

        private string TransactionType;
        #endregion


        public FormPurchaseMultiples(string transactionType = "other")
        {
            TransactionType = transactionType;
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        

        private void FormReceiveMultiple_Load(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PurchaseSearch();
        }


        private void PurchaseSearch()
        {

            try
            {
                #region Check Point

                if (dtpPurchaseFromDate.Value.AddDays(31) < Convert.ToDateTime(dtpPurchaseToDate.Value))
                {
                    dtpPurchaseToDate.Value = dtpPurchaseFromDate.Value.AddDays(31);
                }

                string PurchaseFromDate = "";
                string PurchaseToDate = "";

                string ReceiveNo = txtPurchaseNo.Text.Trim();

                if (string.IsNullOrWhiteSpace(ReceiveNo))
                {
                    PurchaseFromDate = dtpPurchaseFromDate.Value.ToString("yyyy-MMM-dd");
                    PurchaseToDate = dtpPurchaseToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }

                #endregion

                #region Data Call

                PurchaseMasterVM vm = new PurchaseMasterVM();

                string[] cFields = { "pih.PurchaseInvoiceNo", "pih.ReceiveDate>=", "pih.ReceiveDate<", "pih.BranchId", "pih.TransactionType", "SelectTop" };
                string[] cValues =
                    {ReceiveNo, PurchaseFromDate, PurchaseToDate, Program.BranchId.ToString(), TransactionType, "ALL"};

                string[] returnColumns = { "PurchaseInvoiceNo", "ReceiveDate", "Post" };

                DataTable dtReceive = new DataTable();

                dtReceive = _purchaseDAL.SelectAll_Specific(vm, cFields, cValues, returnColumns, null, null,connVM);

                #endregion

                #region Data Load

                dgvPurchaseMultiple.DataSource = null;

                if (dtReceive != null && dtReceive.Rows.Count > 0)
                {
                    dtReceive.Rows.RemoveAt(dtReceive.Rows.Count - 1);

                    dgvPurchaseMultiple.DataSource = dtReceive;
                    LRecordCount.Text = "Record Count :" + dtReceive.Rows.Count.ToString();
                }

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
                FileLogger.Log(this.Name, "ReceiveSearch", exMessage);
            }
            #endregion

        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            SelectAll();
        }


        private void SelectAll()
        {
            foreach (DataGridViewRow dr in dgvPurchaseMultiple.Rows)
            {
                dr.Cells["Select"].Value = chkSelectAll.Checked;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPurchaseNo.Text = "";
            dtpPurchaseFromDate.Value = DateTime.Now;
            dtpPurchaseToDate.Value = DateTime.Now;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            PurchaseUpdate();
        }


        private void PurchaseUpdate()
        {

            try
            {

                this.progressBar1.Visible = true;
                this.pnlButton.Enabled = false;

                List<DataGridViewRow> dgvr = new List<DataGridViewRow>();
                dgvr = dgvPurchaseMultiple.Rows.Cast<DataGridViewRow>().Where(r => Convert.ToBoolean(r.Cells["Select"].Value) == true).ToList();

                vm = new PurchaseMasterVM();
                vm.IDs = new List<string>();
                vm.CurrentUserId = Program.CurrentUserID;
                vm.BranchId = Program.BranchId;

                foreach (DataGridViewRow dr in dgvr)
                {
                    vm.IDs.Add(dr.Cells["PurchaseInvoiceNo"].Value.ToString());
                }

                if (vm.IDs == null || vm.IDs.Count == 0)
                {
                    throw new Exception("Please Select Before Update!");
                }

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

                #region Element Stats

                this.progressBar1.Visible = false;
                this.pnlButton.Enabled = true;

                #endregion
            }
            #endregion

            finally { }

            
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
               


                ResultVM rVM = new ResultVM();

                rVM = _purchaseDAL.MultipleUpdate(vm,null,null,connVM);

                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                

            }
            #region Catch
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion

            finally { }

            

        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                PurchaseSearch();
            }
            #region Catch
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion

            finally { }

            #region Element Stats

            this.progressBar1.Visible = false;
            this.pnlButton.Enabled = true;

            #endregion
            

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            this.pnlButton.Enabled = false;
            progressBar1.Visible = true;
            bgwPost.RunWorkerAsync();

           // ReceivePost();
        }

        private void ReceivePost()
        {
            #region Data Selection

            List<DataGridViewRow> dgvr = new List<DataGridViewRow>();
            dgvr = dgvPurchaseMultiple.Rows.Cast<DataGridViewRow>()
                .Where(r =>
                    Convert.ToBoolean(r.Cells["Select"].Value) == true
                    && Convert.ToString(r.Cells["Post"].Value) == "N")
                .ToList();

            vm = new PurchaseMasterVM();
            vm.IDs = new List<string>();

            foreach (DataGridViewRow dr in dgvr)
            {
                vm.IDs.Add(dr.Cells["PurchaseInvoiceNo"].Value.ToString());
            }

            if (vm.IDs == null || vm.IDs.Count == 0)
            {
                ////throw new Exception("Please Make a Valid Selection Before Post!");
                throw new Exception("All data already posted !");
            }

            #endregion

            #region Data Post

            ResultVM rVM = new ResultVM();
            rVM = _purchaseDAL.MultiplePost(vm);



            #endregion

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            ReceivePost();
        }

        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                PurchaseSearch();

                MessageBox.Show("Posted", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            progressBar1.Visible = false;
            this.pnlButton.Enabled = true;

        }

    }
}
