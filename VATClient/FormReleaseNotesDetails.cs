using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class Details : Form
    {
        public Details()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
      
        //

        public static void ShowDetails(DataGridViewRow row)
        {
            Details details = new Details();

            details.lblName.Text = row.Cells["Name1"].Value.ToString();
            details.lblVersion.Text = row.Cells["Version"].Value.ToString();
            details.lblIssue.Text = row.Cells["Issue"].Value.ToString();
            details.lblDesc.Text = row.Cells["Description"].Value.ToString();
            details.lblDate.Text = Convert.ToDateTime(row.Cells["Date"].Value.ToString()).ToString("dd/MMM/yyyy");


           

            details.ShowDialog();



        }

        private void Details_Load(object sender, EventArgs e)
        {
             DataGridViewRow grow = new DataGridViewRow();
            dgvNotes.Rows.Add(grow);
                dgvNotes["Particular", dgvNotes.RowCount - 1].Value ="Date";
                dgvNotes["Description", dgvNotes.RowCount - 1].Value = lblDate.Text.Trim();

            grow = new DataGridViewRow();
              dgvNotes.Rows.Add(grow);
                dgvNotes["Particular", dgvNotes.RowCount - 1].Value ="Name";
                dgvNotes["Description", dgvNotes.RowCount - 1].Value = lblName.Text.Trim();

            grow = new DataGridViewRow();
            dgvNotes.Rows.Add(grow);
                dgvNotes["Particular", dgvNotes.RowCount - 1].Value ="Version";
                dgvNotes["Description", dgvNotes.RowCount - 1].Value = lblVersion.Text.Trim();
             grow = new DataGridViewRow();
            dgvNotes.Rows.Add(grow);
                dgvNotes["Particular", dgvNotes.RowCount - 1].Value ="Issue";
                dgvNotes["Description", dgvNotes.RowCount - 1].Value = lblIssue.Text.Trim();
                grow = new DataGridViewRow();
                dgvNotes.Rows.Add(grow);
                dgvNotes["Particular", dgvNotes.RowCount - 1].Value ="Description";
                dgvNotes["Description", dgvNotes.RowCount - 1].Value = lblDesc.Text.Trim();


        }
    }
}
