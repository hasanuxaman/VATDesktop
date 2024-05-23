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

namespace VATClient
{
    public partial class FormSaleBombaydetailsSearch : Form
    {
        public FormSaleBombaydetailsSearch()
        {
            InitializeComponent();
        }

        private void FormSaleBombaydetailsSearch_Load(object sender, EventArgs e)
        {
            txtInvoiceNo.Visible = false;

            if (!string.IsNullOrWhiteSpace(txtInvoiceNo.Text.Trim()))
            {
              DataTable  dt = new DataTable();
              SaleDAL saleDal = new SaleDAL();
                //Search(txtChalanNo.Text.Trim());

                 dt = saleDal.SelectAllBSMWDeatailData(txtInvoiceNo.Text.Trim(),null,null,null,null);

                 if (dt != null && dt.Rows.Count > 0)
                 {
                     int RowCount = dt.Rows.Count;
                    // lblRowCount.Text = "Total Invoices: " + RowCount;
                 }

                #region Statement
                // Start Complete
                 dgvSaleBombay.DataSource = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    dgvSaleBombay.DataSource = dt;

                //    //dgvSales.Columns[0].Width = 50;
                //    //dgvSales.Columns[1].Width = 50;
                //    //dgvSales.Columns[2].Width = 150;
                }

                #endregion
            }
        }
    }
}
