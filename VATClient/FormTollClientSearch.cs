using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormTollClientSearch : Form
    {
        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;        
        DataGridViewRow selectedRow = new DataGridViewRow();
        private static string transactionType { get; set; }
        private static string transactionType1 { get; set; }
        string[] IdArray;
        private string SearchBranchId = "0";
        private string RecordCount = "0";
        ResultVM rVM = new ResultVM();
        

        #region Global Variables For BackGroundWorker

        private DataTable TollResult;
        private string cmbPostText;
        string POSTCMB = string.Empty;
        #endregion
        #endregion


        public FormTollClientSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            try
            {
                RecordCount = cmbRecordCount.Text.Trim();

                #region Statement
                this.progressBar1.Visible = true;
                this.btnSearch.Enabled = false;
                //SearchBranchId = cmbBranch.SelectedValue.ToString();
                Search();

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
                FileLogger.Log(this.Name, "btnSearch_Click", exMessage);
            }

            #endregion

        }


        private void Search()
        {
            try
            {
                #region Statement
                POSTCMB = cmbPost.SelectedIndex != -1 ? cmbPost.Text.Trim() : "";
          
                DtpNullChecking();
                this.progressBar1.Visible = true;
                this.btnSearch.Enabled = false;
                backgroundWorkerSearch.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "Search", exMessage);
            }


            #endregion
        }


        string TollClientFromDate, TollClientToDate;
        private void DtpNullChecking()
        {

            try
            {
                #region Statement

                if (dtpTollClientFromDate.Checked == false)
                {
                    TollClientFromDate = "";
                }
                else
                {
                    TollClientFromDate = dtpTollClientFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpTollClientToDate.Checked == false)
                {
                    TollClientToDate = "";
                }
                else
                {
                    TollClientToDate = dtpTollClientToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }


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
                FileLogger.Log(this.Name, "DtpNullChecking", exMessage);
            }
            #endregion

        }

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try
            try
            {
                TollClientInOutDAL tollClientDal = new TollClientInOutDAL();


                string[] cValues = { txtCode.Text.Trim(), TollClientFromDate, TollClientToDate, txtSerialNo.Text.Trim(), txtReceiveNo.Text.Trim(), transactionType, POSTCMB, txtSerialNo1.Text.Trim(), SearchBranchId, RecordCount };
                string[] cFields = { "Code like", "DateTime>", "DateTime<", "SerialNo like", "ReceiveNo like", "TransactionType like", "Post like", "SerialNo like", "BranchId", "SelectTop" };
                TollResult = tollClientDal.SelectAll(0, cFields, cValues, null, null, null, connVM);

                string[] columnNames = { "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn" };

                TollResult = OrdinaryVATDesktop.DtDeleteColumns(TollResult, columnNames);

            }
            #endregion

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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
            }

            #endregion
        }

        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try
            string TotalTecordCount = "0";

            try
            {

                dgvTollClientHistory.DataSource = null;
                if (TollResult != null && TollResult.Rows.Count > 0)
                {

                    TotalTecordCount = TollResult.Rows[TollResult.Rows.Count - 1][0].ToString();
                    TollResult.Rows.RemoveAt(TollResult.Rows.Count - 1);
                    dgvTollClientHistory.DataSource = TollResult;

                    #region Specific Coloumns Visible False

                    
                    dgvTollClientHistory.Columns["Id"].Visible = false;
                    dgvTollClientHistory.Columns["BranchId"].Visible = false;
                    dgvTollClientHistory.Columns["TransactionType"].Visible = false;
                    dgvTollClientHistory.Columns["CustomerID"].Visible = false;
                    

                    #endregion

                }

            }
            #endregion

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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnSearch.Enabled = true;
                LRecordCount.Text = "Total Record Count " + (TollResult.Rows.Count) + " of " + TotalTecordCount.ToString();

            }

        }

        private void dgvTollClientHistory_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                GridSelected();

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
                FileLogger.Log(this.Name, "dgvIssueHistory_DoubleClick", exMessage);
            }


            #endregion
        }

        private void GridSelected()
        {
            try
            {
                #region Statement

                if (dgvTollClientHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvTollClientHistory.SelectedRows;
                if (selectedRows != null && selectedRows.Count > 0)
                {
                    selectedRow = selectedRows[0];
                }

                this.Close();

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
                FileLogger.Log(this.Name, "Search", exMessage);
            }

            #endregion

        }


        public static DataGridViewRow SelectOne(string transType)
        {
            DataGridViewRow selectedRowTemp = null;

            try
            {
                #region Statement

                FormTollClientSearch frmTolleSearch = new FormTollClientSearch();
                transactionType = transType;


                if (transType == "TollCont6_4Outs")
                {

                    frmTolleSearch.Text = "TollCont6_4Outs";
                    transactionType = transType;

                }
                if (transType == "TollCont6_4Ins")
                {

                    frmTolleSearch.Text = "TollCont6_4Ins";
                    transactionType = transType;

                }
                if (transType == "TollCont6_4Backs")
                {

                    frmTolleSearch.Text = "TollCont6_4Backs";
                    transactionType = transType;

                }



                frmTolleSearch.ShowDialog();

                selectedRowTemp = frmTolleSearch.selectedRow;

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
                MessageBox.Show(ex.Message, "FormIssueSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormIssueSearch", "SelectOne", exMessage);
            }
            #endregion

            return selectedRowTemp;
        }


    
    }
}
