using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormErrorMessageIntegration : Form
    {
        public FormErrorMessageIntegration()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        //

        public static void ShowDetails(DataTable table)
        {
            FormErrorMessageIntegration details = new FormErrorMessageIntegration();
            details.dgvMessages.Rows.Clear();
            details.dgvMessages.DataSource = table;

            int invoiceCount = table.DefaultView.ToTable(true, "ID").Rows.Count;

            details.lblRecord.Text = "Invoice Count: " + invoiceCount;

            details.ShowDialog();

        }

        private void Details_Load(object sender, EventArgs e)
        {
           
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DataTable errorTable = (DataTable) dgvMessages.DataSource;

            if (errorTable != null)
            {
                OrdinaryVATDesktop.SaveExcel(errorTable,"ErrorMessage","Messages");
            }
            else
            {
                MessageBox.Show("No Data Found");
            }
        }
    }
}
