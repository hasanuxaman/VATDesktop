using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormReverseAdjustment : Form
    {
        public string transactionType = string.Empty;
        public FormReverseAdjustment()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (rbtnVDS.Checked)
            {
                transactionType = "VDS";
               
            }
            else if (rbtnTreasury.Checked)
            {
                transactionType = "Treasury";
            }
            else if (rbtnSD.Checked)
            {
                transactionType = "SD";
            }
            else if (rbtnAdjustment.Checked)
            {
                transactionType = "AdjCashPayble";
            }

            FormDepositReverse frmDeposit = new FormDepositReverse();
            MDIMainInterface mdi = new MDIMainInterface();
            //mdi.RollDetailsInfo("5401");

            //if (Program.Access != "Y")
            //{
            //    MessageBox.Show("You do not have to access this form", frmDeposit.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            frmDeposit.searchReverseExist(transactionType);
            frmDeposit.Show();

            this.Close();

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FormDepositReverse frmDeposit = new FormDepositReverse();
            //MDIMainInterface mdi = new MDIMainInterface();
            //mdi.RollDetailsInfo("5401");

            //if (Program.Access != "Y")
            //{
            //    MessageBox.Show("You do not have to access this form", frmDeposit.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            if (rbtnVDS.Checked)
            {
                transactionType = "VDS";
                frmDeposit.rbtnVDS.Checked = true;

            }
            else if (rbtnTreasury.Checked)
            {
                transactionType = "Treasury";
                frmDeposit.rbtnTreasury.Checked = true;
            }
            else if (rbtnSD.Checked)
            {
                transactionType = "SD";
                frmDeposit.rbtnSD.Checked = true;
            }
            else if (rbtnAdjustment.Checked)
            {
                transactionType = "AdjCashPayble";
                frmDeposit.rbtnAdjCashPayble.Checked = true;
            }

           
            frmDeposit.Show();

            this.Close();
        }

        private void FormReverseAdjustment_Load(object sender, EventArgs e)
        {

        }
    }
}
