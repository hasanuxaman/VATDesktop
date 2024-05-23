using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class IntegrationComp : Form
    {
        public IntegrationComp()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ImportDAL importDal = new ImportDAL();

                BranchProfileDAL dal = new BranchProfileDAL();
                SaleDAL saleDal = new SaleDAL();
                DataTable dt = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);

                IntegrationParam param = new IntegrationParam
                {
                    TransactionType = "Other",
                    RefNo = txtID.Text,
                    dtConnectionInfo = dt,

                    WithIsProcessed = false
                };

                DataTable salesData = importDal.GetSaleACICompData(param);

                DataTable integrationTable = importDal.GetSaleACIData_Middleware(param);

                var detailData = saleDal.SelectSaleDetail(null,new []{"sd.SalesInvoiceNo"}, new []{salesData.Rows[0]["InvoiceNo"].ToString()},null,null,connVM);

                DataTable detailDt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(detailData));


                DataSet ds = new DataSet("Sale");
                ds.Tables.Add(salesData);
                ds.Tables.Add(integrationTable);
                ds.Tables.Add(detailDt);


                OrdinaryVATDesktop.SaveExcelMultiple(ds, "Products", new[] {"Comparison", "IntegrationTable","SaleDetail"});


            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void IntegrationComp_Load(object sender, EventArgs e)
        {

        }
    }
}
