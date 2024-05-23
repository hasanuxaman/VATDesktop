using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATClient.ReportPreview;

namespace VATClient
{
    public partial class Form12Law : Form
    {
        MdiClient mdi;

        public Form12Law()
        {
            InitializeComponent();
            foreach (Control c in this.Controls)
            {
                if (c is MdiClient)
                {
                    mdi = (MdiClient)c;
                    break;
                }
            }
        }

        private void rBtnBranch_Click(object sender, EventArgs e)
        {

            FormBranchName frmImport = new FormBranchName();

            frmImport.Show();
        }

        private void rBtnBranchReport_Click(object sender, EventArgs e)
        {
            FormBranchReport frmImport = new FormBranchReport();
            frmImport.Show(); 


        }

        private void rBtn6_1in_Click(object sender, EventArgs e)
        {
            FormTransferReceive frm = new FormTransferReceive();
            frm.rbtn61In.Checked = true;

            frm.Show();

        }

        private void rBtn6_2in_Click(object sender, EventArgs e)
        {
            FormTransferReceive frm = new FormTransferReceive();
            frm.rbtn62In.Checked = true;

            frm.Show();
        }

        private void rBtn6_1out_Click(object sender, EventArgs e)
        {
            FormTransferIssue frm = new FormTransferIssue();
            frm.rbtn61Out.Checked = true;
            frm.Show();
        }

        private void rBtn6_2out_Click(object sender, EventArgs e)
        {
            FormTransferIssue frm = new FormTransferIssue();
            frm.rbtn62Out.Checked = true;
            frm.Show();
        }

        private void btn6_5_Click(object sender, EventArgs e)
        {
            FormRptVAT6_5 frm = new FormRptVAT6_5();
            frm.Show();
        }

        private void btn6_10_Click(object sender, EventArgs e)
        {
            FormVAT6_10 frm = new FormVAT6_10();
            frm.Show();
        }

        private void Form12Law_Load(object sender, EventArgs e)
        {



        }




    }
}
