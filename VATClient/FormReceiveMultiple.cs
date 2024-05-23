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
    public partial class FormReceiveMultiple : Form
    {
        #region Declarations
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        ReceiveMasterVM vm = new ReceiveMasterVM();
        ReceiveDAL _ReceiveDAL = new ReceiveDAL();
        private string TransactionType;
        #endregion


        public FormReceiveMultiple(string transactionType = "other")
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
            ReceiveSearch();
        }


        private void ReceiveSearch()
        {

            try
            {
                #region Check Point

                if (dtpReceiveFromDate.Value.AddDays(31) < Convert.ToDateTime(dtpReceiveToDate.Value))
                {
                    dtpReceiveToDate.Value = dtpReceiveFromDate.Value.AddDays(31);
                }

                string ReceiveFromDate = "";
                string ReceiveToDate = "";

                string ReceiveNo = txtReceiveNo.Text.Trim();

                if (string.IsNullOrWhiteSpace(ReceiveNo))
                {
                    ReceiveFromDate = dtpReceiveFromDate.Value.ToString("yyyy-MMM-dd");
                    ReceiveToDate = dtpReceiveToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }

                #endregion

                #region Data Call

                ReceiveMasterVM vm = new ReceiveMasterVM();

                string[] cFields = { "rh.ReceiveNo like", "rh.ReceiveDateTime>=", "rh.ReceiveDateTime<", "rh.BranchId", "rh.TransactionType", "SelectTop" };
                string[] cValues =
                    {ReceiveNo, ReceiveFromDate, ReceiveToDate, Program.BranchId.ToString(), TransactionType, "ALL"};

                string[] returnColumns = { "ReceiveNo", "ReceiveDateTime", "Post" };
                vm.ReceiveOnly = true;

                DataTable dtReceive = new DataTable();

                dtReceive = _ReceiveDAL.SelectAll_Specific(vm, cFields, cValues, returnColumns, null, null,connVM);

                #endregion

                #region Data Load

                dgvReceiveMultiple.DataSource = null;

                if (dtReceive != null && dtReceive.Rows.Count > 0)
                {
                    dtReceive.Rows.RemoveAt(dtReceive.Rows.Count - 1);

                    dgvReceiveMultiple.DataSource = dtReceive;
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
            foreach (DataGridViewRow dr in dgvReceiveMultiple.Rows)
            {
                dr.Cells["Select"].Value = chkSelectAll.Checked;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtReceiveNo.Text = "";
            dtpReceiveFromDate.Value = DateTime.Now;
            dtpReceiveToDate.Value = DateTime.Now;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            ReceiveUpdate();
        }


        private void ReceiveUpdate()
        {

            try
            {

                this.progressBar1.Visible = true;
                this.pnlButton.Enabled = false;

                List<DataGridViewRow> dgvr = new List<DataGridViewRow>();
                dgvr = dgvReceiveMultiple.Rows.Cast<DataGridViewRow>().Where(r => Convert.ToBoolean(r.Cells["Select"].Value) == true).ToList();

                vm = new ReceiveMasterVM();
                vm.IDs = new List<string>();
                vm.CurrentUserID = Program.CurrentUserID;

                foreach (DataGridViewRow dr in dgvr)
                {
                    vm.IDs.Add(dr.Cells["ReceiveNo"].Value.ToString());
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

                rVM = _ReceiveDAL.MultipleUpdate(vm,null,null,connVM);

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
                ReceiveSearch();
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
            ReceivePost();
        }

        private void ReceivePost()
        {
            try
            {
                #region Element Stats

                this.pnlButton.Enabled = false;

                #endregion

                #region Data Selection

                List<DataGridViewRow> dgvr = new List<DataGridViewRow>();
                dgvr = dgvReceiveMultiple.Rows.Cast<DataGridViewRow>()
                        .Where(r =>
                            Convert.ToBoolean(r.Cells["Select"].Value) == true
                            && Convert.ToString(r.Cells["Post"].Value) == "N")
                        .ToList();

                vm = new ReceiveMasterVM();
                vm.IDs = new List<string>();

                foreach (DataGridViewRow dr in dgvr)
                {
                    vm.IDs.Add(dr.Cells["ReceiveNo"].Value.ToString());
                }

                if (vm.IDs == null || vm.IDs.Count == 0)
                {
                    //////throw new Exception("Please Make a Valid Selection Before Post!");
                    throw new Exception("All data already posted !");
                }

                #endregion

                #region Data Post

                ResultVM rVM = new ResultVM();
                rVM = _ReceiveDAL.MultiplePost(vm, null, null,connVM);

                MessageBox.Show(rVM.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                ReceiveSearch();

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
                FileLogger.Log(this.Name, "ReceivePost", exMessage);
            }
            #endregion
            finally { }

            #region Element Stats

            this.pnlButton.Enabled = true;

            #endregion

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
