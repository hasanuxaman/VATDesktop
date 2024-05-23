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
    public partial class FormDBAccess : Form
    {
        public FormDBAccess()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string[] sqlResults;
        private bool IsSymphonyUser = false;
        private DataTable dt = new DataTable();

        private void FormDBAccess_Load(object sender, EventArgs e)
        {
           this.Text= "Data Source=" + SysDBInfoVM.SysdataSource + ";DataBase Name = " + DatabaseInfoVM.DatabaseName;
            if (DialogResult.No != MessageBox.Show("Are you Symphony user?", this.Text, MessageBoxButtons.YesNo,
                                                           MessageBoxIcon.Question,
                                                           MessageBoxDefaultButton.Button2))
            {
                IsSymphonyUser = FormSupperAdministrator.SelectOne();
                if (!IsSymphonyUser)
                {
                    btnExecute.Enabled = false;
                    btnExecuteUpdate.Enabled = false;
                    btnAvgPrice.Enabled = false;
                    btnStock.Enabled = false;
                    btnDbAll.Enabled = false;
                    btnImportStock.Enabled = false;

                    
                }
            }
            else
            {
                btnExecute.Enabled = false;
                btnExecuteUpdate.Enabled = false;

                btnAvgPrice.Enabled = false;
                //btnStock.Enabled = false;
                btnDbAll.Enabled = false;
                btnImportStock.Enabled = false;

            }
        }

        private void TableLoad()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();

                string SqlText = @" SELECT  [Name] TableName FROM   SYSOBJECTS WHERE  xtype = 'U'   ";

                string SQLTextRecordCount = @" select count(Name)RecordNo FROM   SYSOBJECTS WHERE  xtype = 'U'";

                string[] shortColumnName = { "Name" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtTableName.Text = selectedRow.Cells["TableName"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "VendorLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

          
        }
        private void backgroundWorkerExecute_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                // Start DoWork

                CommonDAL _cdal = new CommonDAL();
                sqlResults = _cdal.ExecuteQuery(txtSQLUpdate.Text.Trim(),null,null,connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerExecute_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerUpdate_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                    }
                this.progressBar1.Visible = false;

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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnExecute.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }
        private void SelectQuery()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSQL.Text.Trim()))
                {
                    MessageBox.Show("There have No Query for Execute");
                    return;
                }

                this.progressBar1.Visible = true;
                this.btnExecute.Enabled = false;

                backgroundWorkerSelect.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SelectQuery();
            
        }

        private void backgroundWorkerSelect_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                //BugsBD
                string SQLText = OrdinaryVATDesktop.SanitizeInput(txtSQL.Text.Trim());
                

                dt = new DataTable();
                CommonDAL _cdal = new CommonDAL();
               //dt= _cdal.ExecuteQuerySelect(txtSQL.Text.Trim(),null,null,true,connVM);
                dt = _cdal.ExecuteQuerySelect(SQLText, null, null, true, connVM);

               

 

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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerSelect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete
                dgvUserGroup.DataSource = null;
                dgvUserGroup.Rows.Clear();
                dgvUserGroup.DataSource = dt;
              


                // End Complete

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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                LRecordCount.Text = "Record Count: " + dgvUserGroup.Rows.Count.ToString();
                this.btnExecute.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnExecuteUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSQLUpdate.Text.Trim()))
            {
                MessageBox.Show("There have No Query for Execute");
                return;
            }

            this.progressBar1.Visible = true;
            this.btnExecute.Enabled = false;
            
                backgroundWorkerExecute.RunWorkerAsync();
             
        }

        private void btnExecuteRefresh_Click(object sender, EventArgs e)
        {
            txtSQL.Text = "";
        }

        private void btnExecuteUpdateRefresh_Click(object sender, EventArgs e)
        {
            txtSQLUpdate.Text = "";
        }

        private void txtTableName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
               TableLoad();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {

           
            var sheetNames = new List<string> { "Products" };
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dt);

            OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "ExecutionData", sheetNames.ToArray());
            dataSet.Tables.Remove(dt);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void txtTableName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTableName_DoubleClick(object sender, EventArgs e)
        {
            TableLoad();
        }

        private void txtTableName_Leave(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            CommonDAL _cdal= new CommonDAL();
          dt=  _cdal.SearchTableFields(null, txtTableName.Text.Trim());
          listBox1.DataSource = dt;
          listBox1.DisplayMember = "Name";

        }

        private void btnMultipleSelect_Click(object sender, EventArgs e)
        {
            string strItem="";
            foreach (DataRowView row in listBox1.SelectedItems)
            {
                strItem = strItem + row.Row["name"].ToString() + ",";
            }
            strItem ="Select "+ strItem.Substring(0, strItem.Length-1)+" From "+ txtTableName.Text.Trim();
            txtSQL.Text = strItem;

             

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string strItem = "";
            foreach (DataRowView row in listBox1.SelectedItems)
            {
                strItem = strItem + row.Row["name"].ToString() + ",";
            }
            strItem = "Select " + strItem.Substring(0, strItem.Length - 1) + " From " + txtTableName.Text.Trim();
            txtSQL.Text = strItem;
            SelectQuery();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtTableName.Text = "";
            listBox1.DataSource = null;
            listBox1.Items.Clear();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, true);
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            try
            {
                FormStockProcess formStockProcess = new FormStockProcess();
                formStockProcess.ShowDialog();

                //ResultVM rVM = new ResultVM();

                //ParameterVM vm = new ParameterVM();
                //vm.IsDBMigration = true;
                //vm.BranchId = Program.BranchId;
                //progressBar2.Visible = true;
                //bgwAvgStockProcess.RunWorkerAsync(vm);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btnAvgPrice_Click(object sender, EventArgs e)
        {
            try
            {
                FormIssueMultiple issueMultiple = new FormIssueMultiple();
                issueMultiple.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void bgwAvgStockProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            ProductDAL productDAl = new ProductDAL();
            ParameterVM vm = (ParameterVM)e.Argument;

            ResultVM rVM = productDAl.ProductRefresh(vm,null,null,Program.CurrentUserID,connVM);

            e.Result = rVM;
        }

        private void bgwAvgStockProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if(e.Error == null)
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
                progressBar2.Visible = false;

            }
        }

        private void btnSale_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar2.Visible = true;

                bgwAdjustmentProcess.RunWorkerAsync("sale");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void bgwAdjustmentProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            string flag = e.Argument.ToString();

            if (flag == "sale")
            {
                SaleDAL saleDAl = new SaleDAL();
                ResultVM resultVm = saleDAl.UpdateAdjustmentValue();

                e.Result = resultVm;
            }
            else if (flag == "receive")
            {
                ReceiveDAL receiveDal = new ReceiveDAL();
                ResultVM resultVm = receiveDal.UpdateAdjustmentValue(null,null,null,connVM);

                e.Result = resultVm;
            }
           
        }

        private void bgwAdjustmentProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    ResultVM resultVM = (ResultVM)e.Result;
                    MessageBox.Show(resultVM.Message);
                }
                else
                {
                    MessageBox.Show(e.Error.Message);

                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar2.Visible = false;

            }
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar2.Visible = true;

                bgwAdjustmentProcess.RunWorkerAsync("receive");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnDbAll_Click(object sender, EventArgs e)
        {
            FormDBMigration frm = new FormDBMigration();
            frm.Show();
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

        private void btnImportStock_Click(object sender, EventArgs e)
        {
            try
            {
                FormImportStock form61Process = new FormImportStock();
                form61Process.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

       
    }
}
