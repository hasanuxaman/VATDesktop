using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormSaleInvoice : Form
    {
        private DataTable AvgSalePrice;

        public FormSaleInvoice()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            SaleDAL saleDAL = new SaleDAL();
            //AvgSalePrice = saleDAL.LoadSaleAvgPrice();
            int j = 0;
            dgvAvgPrice.Rows.Clear();
            foreach (DataRow item in AvgSalePrice.Rows)
            {
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvAvgPrice.Rows.Add(NewRow);
                dgvAvgPrice.Rows[j].Cells["LineNoR"].Value = j + 1;
                dgvAvgPrice.Rows[j].Cells["InvoiceNo"].Value = item["SalesInvoiceNo"].ToString();
                dgvAvgPrice.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                dgvAvgPrice.Rows[j].Cells["AvgPrice"].Value = item["AvgPrice"].ToString();
                dgvAvgPrice.Rows[j].Cells["InvoiceDateTime"].Value = item["InvoiceDateTime"].ToString();

                j = j + 1;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvAvgPrice.Rows.Count <= 0)
            {
                MessageBox.Show("There is no data for transaction.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ProductDAL sale=new ProductDAL();

            for (int i = 0; i < dgvAvgPrice.RowCount; i++)
            {
               string vItemNo = dgvAvgPrice["ItemNo", i].Value.ToString();
               DateTime vInvoiceDateTime = Convert.ToDateTime(dgvAvgPrice["InvoiceDateTime", i].Value);
               //decimal newAvgPrice = sale.AvgPrice(vItemNo, vInvoiceDateTime, null, null);
               //dgvAvgPrice["NewAvgPrice", i].Value = Convert.ToDecimal(newAvgPrice);
            }

        }

        private void FormSaleInvoice_Load(object sender, EventArgs e)
        {

        }
    }
}
