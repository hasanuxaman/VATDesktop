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
    public partial class FormTollContSearch : Form
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

        public FormTollContSearch()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        public static DataGridViewRow SelectOne(string transType)
        {
            DataGridViewRow selectedRowTemp = null;

            try
            {
                #region Statement

                FormTollContSearch frmTollContSearch = new FormTollContSearch();
                transactionType = transType;

                if (transType == "TollCont6_4Outs")
                {

                    frmTollContSearch.Text = "TollCont6_4Outs";
                    transactionType = transType;

                }
               

                frmTollContSearch.ShowDialog();

                selectedRowTemp = frmTollContSearch.selectedRow;

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



        private void btnSearch_Click(object sender, EventArgs e)
        {


            try
            {
                RecordCount = cmbRecordCount.Text.Trim();

                #region Statement
                this.progressBar1.Visible = true;
                this.btnSearch.Enabled = false;
                
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

        string TollContFromDate, TollContToDate;
        private void DtpNullChecking()
        {

            try
            {
                #region Statement

                if (dtpTollContFromDate.Checked == false)
                {
                    TollContFromDate = "";
                }
                else
                {
                    TollContFromDate = dtpTollContFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpTollContToDate.Checked == false)
                {
                    TollContToDate = "";
                }
                else
                {
                    TollContToDate = dtpTollContToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
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
                TollContInOutDAL tollDal = new TollContInOutDAL();


                string[] cValues = { txtCode.Text.Trim(), TollContFromDate, TollContToDate, txtSerialNo.Text.Trim(), txtReceiveNo.Text.Trim(), transactionType, POSTCMB, txtSerialNo1.Text.Trim(), SearchBranchId, RecordCount };
                string[] cFields = { "Code like", "DateTime>", "DateTime<", "SerialNo like", "ReceiveNo like", "TransactionType like", "Post like", "SerialNo like", "BranchId", "SelectTop" };
                TollResult = tollDal.SelectAll(0, cFields, cValues, null, null, null, connVM);

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

                dgvTollContHistory.DataSource = null;
                if (TollResult != null && TollResult.Rows.Count > 0)
                {

                    TotalTecordCount = TollResult.Rows[TollResult.Rows.Count - 1][0].ToString();
                    TollResult.Rows.RemoveAt(TollResult.Rows.Count - 1);
                    dgvTollContHistory.DataSource = TollResult;

                    #region Specific Coloumns Visible False


                    dgvTollContHistory.Columns["Id"].Visible = false;
                    dgvTollContHistory.Columns["BranchId"].Visible = false;
                    dgvTollContHistory.Columns["TransactionType"].Visible = false;
                    dgvTollContHistory.Columns["VendorID"].Visible = false;
                    

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

        private void dgvTollContHistory_DoubleClick(object sender, EventArgs e)
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

                if (dgvTollContHistory.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataGridViewSelectedRowCollection selectedRows = dgvTollContHistory.SelectedRows;
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

        private void FormTollContSearch_Load(object sender, EventArgs e)
        {

            try
            {
                cmbRecordCount.DataSource = new RecordSelect().RecordSelectList;

                #region Statement
                string[] Condition = new string[] { "ActiveStatus='Y'" };
                cmbBranch = new CommonDAL().ComboBoxLoad(cmbBranch, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true, true, connVM);
                cmbBranch.SelectedValue = Program.BranchId;

                #region cmbBranch DropDownWidth Change
                string cmbBranchDropDownWidth = new CommonDAL().settingsDesktop("Branch", "BranchDropDownWidth", null, connVM);
                cmbBranch.DropDownWidth = Convert.ToInt32(cmbBranchDropDownWidth);
                #endregion

                if (Program.fromOpen == "Me")
                {
                    //btnAdd.Visible = false;
                }
                //Search();
                cmbExport.SelectedIndex = 0;

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
                FileLogger.Log(this.Name, "FormIssueSearch_Load", exMessage);
            }

            #endregion
        }

        private void FormTollContSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseForm();
        }
        private void CloseForm()
        {
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                if (frm.Name == "FormIssueMultiple")
                {
                    frm.Close();
                    return;
                }
            }

        }








    }
}
