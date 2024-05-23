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
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormSaleTempProcess : Form
    {
        public FormSaleTempProcess()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private DataTable unProcessedData;
        private string[] SqlResult;

        private void btnLocal_Click(object sender, EventArgs e)
        {
            bgwGetUnprocessedData.RunWorkerAsync();
        }

        private void bgwGetUnprocessedData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var saleDal = new SaleDAL();
               // pbProcess.Visible = true;
                unProcessedData = saleDal.GetUnProcessedTempData(connVM);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void bgwGetUnprocessedData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                LUnprocessed.Text = @"Total Unprocessed Data = " + unProcessedData.Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
               // pbProcess.Visible = false;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            var message = "Are you sure to process temp data?";
            var caption = "Sale temp data process";
            var buttons = MessageBoxButtons.YesNo;

            var result = MessageBox.Show(message, caption, buttons);

            if(result == DialogResult.No)
                return;

            if (result == DialogResult.Yes)
            {
                bgwProcessTempData.RunWorkerAsync();
            }
 
        }

        private void bgwProcessTempData_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var saleDal = new SaleDAL();

                SqlResult = saleDal.ProccessTempData(connVM);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bgwProcessTempData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SqlResult[0].ToLower() == "success")
            {
                MessageBox.Show("Data Processed");
            }
        }

        private void FormSaleTempProcess_Load(object sender, EventArgs e)
        {

        }
    }
}
