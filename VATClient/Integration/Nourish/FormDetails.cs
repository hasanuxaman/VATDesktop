using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VATClient.Integration.Nourish
{
    public partial class FormDetails : Form
    {
        public FormDetails()
        {
            InitializeComponent();
        }

        private void FormDetails_Load(object sender, EventArgs e)
        {

        }


        public static void SelectOne(DataTable dt)
        {
            FormDetails form = new FormDetails();

            form.dgvDetails.DataSource = dt;
 
            form.ShowDialog();
        }
    }
}
