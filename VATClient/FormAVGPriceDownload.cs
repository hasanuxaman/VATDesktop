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
    public partial class FormAVGPriceDownload : Form
    {
        private DataTable dt;

        public FormAVGPriceDownload()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            try
            {
                

                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerSearch.RunWorkerAsync();


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

        private void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ProductDAL dal = new ProductDAL();
                dt = dal.SelectAllAVGPrice(null,null,null,null,null,true,connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                dgvCustomer.DataSource = null;
                if (dt != null && dt.Rows.Count > 0)
                {

                    #region Specific Coloumns Visible False

                    dgvCustomer.DataSource = dt;
                    dgvCustomer.Columns["ItemNo"].Visible = false;

                    #endregion
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
                FileLogger.Log(this.Name, "backgroundWorkerSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                //LRecordCount.Text = "Record Count: " + dgvCustomer.Rows.Count.ToString();
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
                LRecordCount.Text = "Total Record Count " + (dt.Rows.Count);

            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            try
            {
                var ids = new List<string>();

                for (int i = 0; i < dgvCustomer.RowCount; i++)
                {
                    if (Convert.ToBoolean(dgvCustomer["Select", i].Value))
                    {
                        ids.Add(dgvCustomer["ItemNo", i].Value.ToString());
                    }
                }

                var pDal = new ProductDAL();


                if (ids.Count == 0)
                {
                    MessageBox.Show("No Data Found");
                    return;
                }

                DataTable dt = pDal.GetExcelDataAVGPrice(ids, null, null, connVM);

                var dataSet = new DataSet();
                dataSet.Tables.Add(dt);

                var sheetNames = new[] { "ProductAVGPrice" };

                OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "ProductAVGPrice", sheetNames);



                #region temp

                
                #endregion


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvCustomer.RowCount; i++)
            {
                dgvCustomer["Select", i].Value = chkSelectAll.Checked;
            }
        }

        private void FormAVGPriceDownload_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

    }
}
