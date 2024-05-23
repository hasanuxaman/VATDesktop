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

namespace VATClient.ReportPreview
{
    public partial class FormBranchStockMovement : Form
    {
        public FormBranchStockMovement()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();

        }
        public static string vItemNo = "0";
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {
            vItemNo = "";
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductLoad();
            }
        }
        private void ProductLoad()
        {
            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();


                                    string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
                     left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
                    where 1=1 and  p.ActiveStatus='Y'  ";
                    string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                    string tableName = "";
                    FormMultipleSearch frm = new FormMultipleSearch();
                    selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName,"", shortColumnName);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                        txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();


                    }
               
 
               
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void FormBranchStockMovement_Load(object sender, EventArgs e)
        {

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            string[] Condition = new string[] { "one" };
            try
            {

            
                var productDal = new TransferIssueDAL();

                var fromDate = dtpFromDate.Text;
                var toDate = dtpToDate.Text;


                var tables = productDal.TransferMovement(vItemNo, fromDate, toDate,Program.BranchId,null,null,chkSummery.Checked,connVM);
                var branch = new BranchProfileDAL().SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);

                var BranchLegalName = branch.Rows[0]["BranchLegalName"].ToString();


                Condition = new string[3];
                Condition[0] = Program.CompanyName;
                Condition[1] = "Date From: " + dtpFromDate.Value.ToString("dd/MMM/yyyy") + " To " + dtpToDate.Value.ToString("dd/MMM/yyyy");
                if (chkSummery.Checked)
                {
                    Condition[2] = BranchLegalName+" Branch Stock Movement(Summery)";

                }
                else
                {
                    Condition[2] = BranchLegalName+" Branch Stock Movement(Details)";
                }
                OrdinaryVATDesktop.SaveExcelMultiple(tables, "BranchStockMovement", new[] { "StockMovement" }, Condition);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDownload", ex.Message);
            }
        }
    }
}
