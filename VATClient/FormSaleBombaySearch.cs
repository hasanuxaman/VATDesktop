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

namespace VATClient
{
    public partial class FormSaleBombaySearch : Form
    {
        private string RecordCount = "0";
        private DataTable SaleBombayResult;
        private string FromDate, ToDate;
        private string IsProcessed = string.Empty;

        public FormSaleBombaySearch()
        {
            InitializeComponent();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //RecordCount = cmbRecordCount.Text.Trim();
            if (dtpDateFrom.Checked == false)
            {
                FromDate = "";
            }
            else
            {
                FromDate = dtpDateFrom.Value.ToString("yyyy-MM-dd");
            }
            if (dtpDateTo.Checked == false)
            {
                ToDate = "";
            }
            else
            {
                ToDate = dtpDateTo.Value.ToString("yyyy-MM-dd");
            }


            Search();
        }

        private void Search()
        {

           
            try
            {
                IsProcessed = cmbIsProcessed.SelectedIndex != -1 ? cmbIsProcessed.Text.Trim() : string.Empty;
                this.btnSearch.Enabled = false;
                this.progressBar1.Visible = true;

                dgvSaleBombay.Columns[0].Width = 50;
                dgvSaleBombay.Columns[1].Width = 50;

                bgwSearch.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "Search", exMessage);
            }
            #endregion Catch

        }

        private void bgwSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                
                
                SaleBombayResult = new DataTable();
                SaleDAL saleDal = new SaleDAL();

                string[] cValues = { txtInvoiceNo.Text.Trim(), IsProcessed, FromDate, ToDate };
                string[] cFields = { "ID like", "IsProcessed", "Invoice_Date>=", "Invoice_Date<=" };

                SaleBombayResult = saleDal.SelectAllBSMwareData(0, cFields, cValues, null, null,null,false,null,null);
                

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
                FileLogger.Log(this.Name, "bgwSearch_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string TotalTecordCount = "0";

            try
            {
                #region Statement

                // Start Complete

                dgvSaleBombay.DataSource = null;
                if (SaleBombayResult != null && SaleBombayResult.Rows.Count > 0)
                {

                   // TotalTecordCount = SaleBombayResult.Rows[SaleBombayResult.Rows.Count - 1][0].ToString();

                    //SaleBombayResult.Rows.RemoveAt(SaleBombayResult.Rows.Count - 1);

                    dgvSaleBombay.DataSource = SaleBombayResult;
                  //  dgvSaleBombay.Columns["ID"].Visible = true;

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
                FileLogger.Log(this.Name, "bgwSearch_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnSearch.Enabled = true;
                this.progressBar1.Visible = false;
                LRecordCount.Text = "Total Record Count " + (dgvSaleBombay.Rows.Count) + " of " + TotalTecordCount.ToString();

            }
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvSaleBombay.RowCount; i++)
            {
                dgvSaleBombay["Select", i].Value = chkSelectAll.Checked;
            }
        }

        private void dgvSaleBombay_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;


            if (e.ColumnIndex == 1)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.code.Width;
                var h = Properties.Resources.code.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;
                e.Graphics.DrawImage(Properties.Resources.code, new Rectangle(x, y, w, h));

                e.Handled = true;
            }
        }

        private void dgvSaleBombay_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView senderGrid = (DataGridView)sender;
                string InvoiceNo = dgvSaleBombay.CurrentRow.Cells["ID"].Value.ToString();

                if (e.ColumnIndex == 1)
                {

                    FormSaleBombaydetailsSearch frm = new FormSaleBombaydetailsSearch();
                    frm.txtInvoiceNo.Text = InvoiceNo;
                    frm.ShowDialog();

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FormSaleBombaySearch_Load(object sender, EventArgs e)
        {

        }

    }
}
