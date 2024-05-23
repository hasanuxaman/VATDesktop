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
    public partial class FormNegativeDownload : Form
    {

        private readonly SysDBInfoVMTemp connVM;

        public FormNegativeDownload()
        {
            InitializeComponent();

            connVM = Program.OrdinaryLoad();
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;


            await Task.Run(() =>
            {

                DataLoad_61();
            });

            progressBar1.Visible = false;

        }


        private void DataLoad_61()
        {
            ProductDAL dal = new ProductDAL();
            DataTable dt = new DataTable();
            DataTable Branchdt = new DataTable();
            DataSet ds = dal.SelectNegInventoryData("6_1", null, null, null, null, connVM);

            var dataSet = new DataSet();
            dt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[0].Copy());
            dataSet.Tables.Add(dt);

            Branchdt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[1].Copy());
            dataSet.Tables.Add(Branchdt);

            var sheetNames = new[] { "VAT_6_1_Negetive", "VAT_6_1_BranchWiseNegetive" };

            OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "VAT_6_1_Negetive", sheetNames);
        }


        private void DataLoad_62()
        {

            ProductDAL dal = new ProductDAL();
            DataTable dt = new DataTable();
            DataTable Branchdt = new DataTable();
            DataSet ds = dal.SelectNegInventoryData("6_2", null, null, null, null, connVM);

            var dataSet = new DataSet();
            dt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[0].Copy());
            dataSet.Tables.Add(dt);
            Branchdt = OrdinaryVATDesktop.DtSlColumnAdd(ds.Tables[1].Copy());
            dataSet.Tables.Add(Branchdt);

            var sheetNames = new[] { "VAT_6_2_Negetive", "VAT_6_2_BranchWiseNegetive" };

            OrdinaryVATDesktop.SaveExcelMultiple(dataSet, "VAT_6_2_Negetive", sheetNames);
        }

        private async void btn62NegDownload_Click(object sender, EventArgs e)
        {

            progressBar1.Visible = true;


            await Task.Run(() =>
            {
                DataLoad_62();
            });

            progressBar1.Visible = false;

           
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
