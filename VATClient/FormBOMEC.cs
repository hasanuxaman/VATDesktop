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
    public partial class FormBOMEC : Form
    {
        public FormBOMEC()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        DataTable BOMRaws = new DataTable();
        DataTable BOMs = new DataTable();
        DataTable BOMCompanyOverhead = new DataTable();

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

           
            CommonDAL _cdal = new CommonDAL();

            BOMRaws = _cdal.ExecuteQuerySelect("select * from BOMRaws");
            BOMCompanyOverhead = _cdal.ExecuteQuerySelect("select * from BOMCompanyOverhead");
            BOMs = _cdal.ExecuteQuerySelect("select * from BOMs  where FinishItemNo not in('1')");

            foreach (DataRow BOMsdr in BOMs.Rows) // search whole table
            {
                string BOMId = BOMsdr["BOMId"].ToString();
                string FinishItemNo = BOMsdr["FinishItemNo"].ToString();
                foreach (DataRow BOMRawsdr in BOMRaws.Rows) // search whole table
                {
                    BOMRawsdr["BOMId"] = BOMId;
                    BOMRawsdr["FinishItemNo"] = FinishItemNo;
                }
                var varBOMRaws = _cdal.BulkInsert("BOMRaws", BOMRaws,null,null,0,null,connVM);

                foreach (DataRow BOMRawsdr in BOMCompanyOverhead.Rows) // search whole table
                {
                    BOMRawsdr["BOMId"] = BOMId;
                    BOMRawsdr["FinishItemNo"] = FinishItemNo;
                }
                var varBOMCompanyOverhead = _cdal.BulkInsert("BOMCompanyOverhead", BOMCompanyOverhead,null,null,0,null,connVM);

            }
             



            }
            catch (Exception Ex)
            {

                throw Ex;
            }
          
            
        }

        private void FormBOMEC_Load(object sender, EventArgs e)
        {

        }
    }
}
