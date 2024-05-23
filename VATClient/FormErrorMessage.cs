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
    public partial class FormErrorMessage : Form
    {
        public FormErrorMessage()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        //

        public static void ShowDetails(List<ErrorMessage> messages)
        {
            FormErrorMessage details = new FormErrorMessage();
            details.dgvMessages.Rows.Clear();
            int i = 0;
            foreach (ErrorMessage msg in messages)
            {
                DataGridViewRow grow = new DataGridViewRow();

                details.dgvMessages.Rows.Add(grow);

                details.dgvMessages["Particular", i].Value = msg.ColumnName;
                details.dgvMessages["Description", i].Value = msg.Message;

                i++;
            }



            details.ShowDialog();

        }


        public void ShowDetails_NS(List<ErrorMessage> messages)
        {
            dgvMessages.Rows.Clear();
            int i = 0;
            foreach (ErrorMessage msg in messages)
            {
                DataGridViewRow grow = new DataGridViewRow();

                dgvMessages.Rows.Add(grow);

                dgvMessages["Particular", i].Value = msg.ColumnName;
                dgvMessages["Description", i].Value = msg.Message;

                i++;
            }
        }

        private void Details_Load(object sender, EventArgs e)
        {
           
        }
    }
}
